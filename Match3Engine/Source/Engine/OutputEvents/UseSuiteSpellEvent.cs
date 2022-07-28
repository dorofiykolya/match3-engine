namespace Match3.Engine.OutputEvents
{
  public class UseSuiteSpellEvent : OutputEvent
  {
    public int Id;
    public int Level;

    public override void Reset()
    {
      Id = Level = 0;
    }
  }
}
