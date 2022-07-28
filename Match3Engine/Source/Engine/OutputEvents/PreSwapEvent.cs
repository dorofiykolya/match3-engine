using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class PreSwapEvent : OutputEvent
  {
    public Swap Swap;
    public int Total;
    public int Used;

    public override void Reset()
    {
      Swap = null;
      Total = 0;
      Used = 0;
    }
  }
}
