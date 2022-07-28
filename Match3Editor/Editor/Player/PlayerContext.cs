using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Editor.Utils.Coroutine;

namespace Match3.Editor.Player
{
  public class PlayerContext
  {
    private readonly LinkedList<IEnumerator> _actions = new LinkedList<IEnumerator>();

    public PlayerContext(CoroutineManager coroutineManager)
    {
      CoroutineManager = coroutineManager;
      coroutineManager.StartCoroutine(ExecuteActions());
    }

    public bool QueueIsEmpty { get { return _actions.Count == 0; } }

    public CoroutineManager CoroutineManager { get; private set; }
    public ITimeProvider TimeProvider { get { return CoroutineManager; } }

    public void Enqueue(IEnumerator action)
    {
      _actions.AddLast(action);
    }

    public Sequence CreateSequence()
    {
      return new Sequence(CoroutineManager);
    }

    private IEnumerator ExecuteActions()
    {
      while (true)
      {
        while (_actions.Count != 0)
        {
          var action = _actions.First;
          if (action.Value.MoveNext())
          {
            yield return action.Value.Current;
          }
          else
          {
            _actions.RemoveFirst();
          }
        }
        yield return null;
      }
    }
  }
}
