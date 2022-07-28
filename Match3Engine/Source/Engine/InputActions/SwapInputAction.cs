using Match3.Engine.Levels;

namespace Match3.Engine.InputActions
{
  public class SwapInputAction : InputAction
  {
    public Swap Swap;

    public override string ToString()
    {
      return string.Format("[{0}][{1}] Swap:{2}", Tick, GetType().Name, Swap);
    }
  }
}
