using System;
using System.Collections.Generic;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;

namespace Match3.Engine.Matches
{
  public class Match2WithUniversalSwapItemCombination : MatchCombination
  {
    public Match2WithUniversalSwapItemCombination(int priority) : base(priority, null)
    {
      AddPattern("[#|#]");

      AddPattern("[#]" +
                 "[#]");
    }

    public override bool Match(Swap swap, ITileGridProvider grid, MatchCombinationsResult result = null)
    {
      var first = grid.GetTile(swap.First);
      var second = grid.GetTile(swap.Second);

      if (first.Item == null || second.Item == null) throw new InvalidOperationException("невозможно перемещать пустые ячейки");

      if (first.ItemType != ItemType.UniversalSwapCell && second.ItemType != ItemType.UniversalSwapCell) return false;

      if (result != null)
      {
        result.AddMatch(new Match(first.Position, new List<Point>(new Point[] { first.Position, second.Position }), Priority));
        result.AddMatch(new Match(second.Position, new List<Point>(new Point[] { first.Position, second.Position }), Priority));
      }

      return true;
    }

    public override bool Match(Point tilePosition, ITileGridProvider grid, MatchCombinationsResult result = null)
    {
      return false;
    }

    protected override bool ItemsEquals(Tile pivot, Tile other)
    {
      return pivot.ItemType == ItemType.Cell && other.Item != null && (other.ItemType == ItemType.UniversalSwapCell || pivot.ItemType == ItemType.UniversalSwapCell);
    }
  }
}
