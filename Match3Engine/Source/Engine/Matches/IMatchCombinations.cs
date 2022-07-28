using Match3.Engine.Levels;

namespace Match3.Engine.Matches
{
  public interface IMatchCombinations
  {
    bool Match(Swap swap, ITileGridProvider grid, MatchCombinationsResult result = null);
    bool Match(Point tilePosition, ITileGridProvider grid, MatchCombinationsResult result = null);
  }
}
