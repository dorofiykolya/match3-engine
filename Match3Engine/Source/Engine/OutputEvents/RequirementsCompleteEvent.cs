namespace Match3.Engine.OutputEvents
{
  public class RequirementsCompleteEvent : OutputEvent
  {
    public int AvailableSwaps;

    public override void Reset()
    {
      AvailableSwaps = 0;
    }
  }
}
