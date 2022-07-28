namespace Match3.Engine.OutputEvents
{
  public class ContinueEvent : OutputEvent
  {
    public int AdditionalSwaps;
    public int Used;
    public int Total;

    public override void Reset()
    {
      AdditionalSwaps = 0;
      Used = 0;
      Total = 0;
    }
  }
}
