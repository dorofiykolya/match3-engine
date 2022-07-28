using System;

namespace Match3.Editor.Utils.Coroutine
{
  public class CoroutineTick
  {
    public event Action<double> Tick;

    public void RaiseTick(double deltaTime)
    {
      if (Tick != null) Tick(deltaTime);
    }
  }
}
