using System;
using System.Collections.Generic;
using System.Reflection;
using Match3.Engine.OutputEvents;

namespace Match3.Engine
{
  public class EngineOutput : IEngineOutput
  {
    private readonly Dictionary<Type, Stack<OutputEvent>> _pool;
    private readonly Queue<OutputEvent> _queue;

    public EngineOutput()
    {
      _pool = new Dictionary<Type, Stack<OutputEvent>>();
      _queue = new Queue<OutputEvent>();
    }

    public bool IsEmpty
    {
      get { return Count == 0; }
    }

    public int Count
    {
      get { return _queue.Count; }
    }

    public OutputEvent Dequeue()
    {
      if (_queue.Count == 0) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": очередь пуста");
      return _queue.Dequeue();
    }

    public void Enqueue(OutputEvent evt)
    {
      _queue.Enqueue(evt);
    }

    public T EnqueueByFactory<T>(int tick) where T : OutputEvent, new()
    {
      Stack<OutputEvent> stack;
      if (_pool.TryGetValue(typeof(T), out stack) && stack.Count != 0)
      {
        var result = (T)stack.Pop();
        result.Tick = tick;
        result.Reset();
        Enqueue(result);
        return result;
      }
      var instance = new T();
      instance.Tick = tick;
      Enqueue(instance);
      return instance;
    }

    public void ReleaseToPool(OutputEvent evt)
    {
      Stack<OutputEvent> stack;
      if (!_pool.TryGetValue(evt.GetType(), out stack))
      {
        _pool[evt.GetType()] = stack = new Stack<OutputEvent>();
      }
      if (stack.Contains(evt)) throw new ArgumentException("данный ивент уже находится в пуле");
      stack.Push(evt);
    }
  }
}
