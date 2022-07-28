namespace Match3.Engine.Matches
{
  public class Match3HorizontalCombination : MatchCombination
  {
    public Match3HorizontalCombination(int priority) : base(priority, null)
    {
      //3x1
      AddPattern(@"[#|#|#]");
    }
  }
}
