using Match3.Engine.Levels;
using Match3.Engine.Matches;
using Match3.Engine.Providers;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedMatchesProvider : IMatchesProvider
  {
    private readonly IMatchCombinations[] _verticalMatches;
    private readonly IMatchCombinations[] _horizontalMatches;
    private readonly IMatchCombinations[] _fastMatches;

    public SharedMatchesProvider()
    {
      _verticalMatches = new IMatchCombinations[]
      {
        new Match2WithUniversalSwapItemCombination(9),
        new Match5Combination(8),
        new Match2With2Level2SwapItemCombination(7),
        new Match4LTCombination(6),
        new Match4VerticalCombination(5),
        new Match4HorizontalCombination(4),
        new Match3TCombination(3),
        new Match3LCombination(2),
        new Match3VerticalCombination(1),
        new Match3HorizontalCombination(0),
      };

      _horizontalMatches = new IMatchCombinations[]
      {
        new Match2WithUniversalSwapItemCombination(9),
        new Match5Combination(8),
        new Match2With2Level2SwapItemCombination(7),
        new Match4LTCombination(6),
        new Match4HorizontalCombination(5),
        new Match4VerticalCombination(4),
        new Match3TCombination(3),
        new Match3LCombination(2),
        new Match3HorizontalCombination(1),
        new Match3VerticalCombination(0),
      };

      _fastMatches = new IMatchCombinations[]
      {
        new Match2WithUniversalSwapItemCombination(3),
        new Match2With2Level2SwapItemCombination(2),
        new Match3HorizontalCombination(1),
        new Match3VerticalCombination(0),
      };
    }

    public bool Match(Swap swap, ITileGridProvider grid, MatchCombinationsResult result = null)
    {
      IMatchCombinations[] combinations = GetMatches(swap.Orientation);
      foreach (var match in combinations)
      {
        if (match.Match(swap, grid, result))
        {
          return true;
        }
      }
      return false;
    }

    public bool Match(Point tilePosition, ITileGridProvider grid, MatchCombinationsResult result = null)
    {
      IMatchCombinations[] combinations = GetMatches(Orientation.Vertical);
      foreach (var match in combinations)
      {
        if (match.Match(tilePosition, grid, result))
        {
          return true;
        }
      }
      return false;
    }

    IMatchCombinations[] GetMatches(Orientation orientation)
    {
      return orientation == Orientation.Horizontal ? _horizontalMatches : _verticalMatches;
    }

    public bool FastMatch(Swap swap, ITileGridProvider grid)
    {
      foreach (var match in _fastMatches)
      {
        if (match.Match(swap, grid))
        {
          return true;
        }
      }
      return false;
    }
  }
}
