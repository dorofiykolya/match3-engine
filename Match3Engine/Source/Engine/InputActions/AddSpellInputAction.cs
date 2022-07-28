namespace Match3.Engine.InputActions
{
  public class AddSpellInputAction : InputAction
  {
    public int Id;
    public int Level;
    public int Count;

    public override string ToString()
    {
      return string.Format("[{0}][{1}] Id:{2}, Level:{3}, Count:{4}", Tick, GetType().Name, Id, Level, Count);
    }
  }
}
