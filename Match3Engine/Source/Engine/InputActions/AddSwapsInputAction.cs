namespace Match3.Engine.InputActions
{
  public class AddSwapsInputAction : InputAction
  {
    public int Swaps;

    public override string ToString()
    {
      return string.Format("[{0}][{1}] Swaps:{2}", Tick, GetType().Name, Swaps);
    }
  }
}
