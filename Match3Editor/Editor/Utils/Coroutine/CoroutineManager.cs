using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Editor.Utils.Coroutine
{
  public class CoroutineManager : ITimeProvider
  {
    private readonly Dictionary<Coroutine, CoroutineStatus> _enumerators = new Dictionary<Coroutine, CoroutineStatus>();

    public double TimeScale { get; set; }
    public double DeltaTime { get; private set; }

    public CoroutineManager(CoroutineTick tickHandler)
    {
      tickHandler.Tick += Tick;
      TimeScale = 1.0;
    }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
      var coroutine = new Coroutine(StopCoroutine);
      _enumerators[coroutine] = new CoroutineStatus(enumerator);
      return coroutine;
    }

    public void StopCoroutine(Coroutine coroutine)
    {
      _enumerators.Remove(coroutine);
    }

    private void Tick(double deltaTime)
    {
      DeltaTime = deltaTime * TimeScale;
      foreach (var pair in _enumerators.ToList())
      {
        var enumerator = pair.Value;
        if (!enumerator.MoveNext())
        {
          pair.Key.Terminate();
        }
      }
    }

    private class CoroutineStatus
    {
      private IEnumerator _current;

      public CoroutineStatus(IEnumerator enumerator)
      {
        _current = enumerator;
      }

      public bool MoveNext()
      {
        var result = _current.MoveNext();
        if (result)
        {
          var current = _current.Current;
          if (current is Coroutine)
          {
            var coroutine = current as Coroutine;
            _current = new CoroutineWrapper(coroutine, _current);
          }
          else if (current is YieldCoroutine)
          {
            var yieldCoroutine = current as YieldCoroutine;
            _current = new YieldWrapper(yieldCoroutine, _current);
          }
        }
        return result;
      }
    }

    private class YieldWrapper : IEnumerator
    {
      private readonly YieldCoroutine _yieldCoroutine;
      private readonly IEnumerator _parent;

      public YieldWrapper(YieldCoroutine yieldCoroutine, IEnumerator parent)
      {
        _yieldCoroutine = yieldCoroutine;
        _parent = parent;
      }

      public bool MoveNext()
      {
        if (_yieldCoroutine.NeedToSkip)
        {
          return true;
        }
        return _parent.MoveNext();
      }

      public void Reset()
      {
        _parent.Reset();
      }

      public object Current { get { return _parent.Current; } }
    }

    private class CoroutineWrapper : IEnumerator
    {
      private readonly Coroutine _coroutine;
      private readonly IEnumerator _parent;

      public CoroutineWrapper(Coroutine coroutine, IEnumerator parent)
      {
        _coroutine = coroutine;
        _parent = parent;
      }

      public bool MoveNext()
      {
        if (!_coroutine.IsTerminated)
        {
          return true;
        }
        return _parent.MoveNext();
      }

      public void Reset()
      {
        _parent.Reset();
      }

      public object Current { get { return _parent.Current; } }
    }

  }
}
