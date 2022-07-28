using System.Collections;
using System.Collections.Generic;

namespace Match3.Editor.Utils.Coroutine
{
  public class Sequence : IEnumerator
  {
    private readonly CoroutineManager _coroutineManager;
    private readonly List<IEnumerator> _collection = new List<IEnumerator>();
    private readonly List<Coroutine> _coroutines = new List<Coroutine>();
    private bool _initialized;

    public Sequence(CoroutineManager coroutineManager)
    {
      _coroutineManager = coroutineManager;
    }

    public void Add(IEnumerator enumerator)
    {
      _collection.Add(enumerator);
    }

    public bool MoveNext()
    {
      if (!_initialized)
      {
        _initialized = true;

        foreach (var enumerator in _collection)
        {
          _coroutines.Add(_coroutineManager.StartCoroutine(enumerator));
        }
        _collection.Clear();
      }

      foreach (var coroutine in _coroutines.ToArray())
      {
        if (coroutine.IsTerminated)
        {
          _coroutines.Remove(coroutine);
        }
      }

      return _coroutines.Count != 0;
    }

    public void Reset()
    {

    }

    public object Current { get { return null; } }
  }
}
