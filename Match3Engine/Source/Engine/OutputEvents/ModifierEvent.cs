using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class ModifierEvent : OutputEvent
  {
    public int Id;
    public int Level;
    public Point Position;

    public override void Reset()
    {
      Id = 0;
      Level = 0;
    }
  }
}
