using System;

namespace Match3.Editor.Utils.Coroutine
{
  public class Coroutine
  {
    public static int _instances;

    private readonly int _id;
    private Action<Coroutine> _terminate;

    public Coroutine(Action<Coroutine> terminate)
    {
      _terminate = terminate;
      _id = ++_instances;
    }

    public int Id { get { return _id; } }
    public bool IsTerminated { get { return _terminate == null; } }

    public void Terminate()
    {
      if (_terminate != null)
      {
        _terminate(this);
        _terminate = null;
      }
    }
  }
}
