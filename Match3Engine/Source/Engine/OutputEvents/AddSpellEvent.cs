namespace Match3.Engine.OutputEvents
{
  public class AddSpellEvent : OutputEvent
  {
    public int Id;
    public int Level;
    public int Count;

    public override void Reset()
    {
      Id = Level = Count = 0;
    }
  }
}
