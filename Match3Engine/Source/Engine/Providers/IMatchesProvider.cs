using Match3.Engine.Levels;
using Match3.Engine.Matches;

namespace Match3.Engine.Providers
{
  public interface IMatchesProvider : IMatchCombinations
  {
    bool FastMatch(Swap swap, ITileGridProvider grid);
  }
}
