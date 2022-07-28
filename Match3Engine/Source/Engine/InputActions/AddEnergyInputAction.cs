namespace Match3.Engine.InputActions
{
  public class AddEnergyInputAction : InputAction
  {
    public int Energy;

    public override string ToString()
    {
      return string.Format("[{0}][{1}] Energy:{2}", Tick, GetType().Name, Energy);
    }
  }
}
