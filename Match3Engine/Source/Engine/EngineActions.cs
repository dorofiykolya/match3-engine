using Match3.Engine.InputActions;
using Match3.Engine.Utils;

namespace Match3.Engine
{
  public class EngineActions : IEngineActions
  {
    private readonly PriorityQueueComparable<InputAction> _actionQueue;

    public EngineActions()
    {
      _actionQueue = new PriorityQueueComparable<InputAction>();
    }

    public int Count
    {
      get { return _actionQueue.Count; }
    }

    public InputAction Peek()
    {
      return _actionQueue.Peek();
    }

    public InputAction Dequeue()
    {
      return _actionQueue.Dequeue();
    }

    public void Enqueue(InputAction action)
    {
      _actionQueue.Enqueue(action);
    }
  }
}
