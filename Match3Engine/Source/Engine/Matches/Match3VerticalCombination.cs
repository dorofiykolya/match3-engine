namespace Match3.Engine.Matches
{
  public class Match3VerticalCombination : MatchCombination
  {
    public Match3VerticalCombination(int priority) : base(priority, null)
    {
      //1x3
      AddPattern(@"[#]" +
                  "[#]" +
                  "[#]");
    }
  }
}
