namespace Match3.Engine.OutputEvents
{
  public class ScoreEvent : OutputEvent
  {
    public int Score;

    public override void Reset()
    {
      Score = 0;
    }
  }
}
