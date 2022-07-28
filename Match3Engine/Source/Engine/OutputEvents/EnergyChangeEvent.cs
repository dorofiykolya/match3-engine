namespace Match3.Engine.OutputEvents
{
  public class EnergyChangeEvent : OutputEvent
  {
    public int Energy;

    public override void Reset()
    {
      Energy = 0;
    }
  }
}
