using System.Collections.Generic;
using Match3.Engine.Levels;
using System;
using Match3.Engine.Descriptions.Items;

namespace Match3.Engine.Matches
{
  public abstract class MatchCombination : IMatchCombinations
  {
    private readonly int _priority;
    private readonly IDictionary<int, Item> _generateItem;
    private readonly MatchPattern _pattern;

    protected MatchCombination(int priority, IDictionary<int, Item> generateItem)
    {
      _priority = priority;
      _generateItem = generateItem;
      _pattern = new MatchPattern();
    }

    public void AddPattern(string pattern)
    {
      _pattern.AddPattern(pattern);
    }

    public IEnumerable<Point[]> PatternOffsets
    {
      get { return _pattern.Offsets; }
    }

    public virtual bool Match(Swap swap, ITileGridProvider grid, MatchCombinationsResult result = null)
    {
      var first = grid.GetTile(swap.First);
      var second = grid.GetTile(swap.Second);

      if (first.Item == null || second.Item == null) throw new InvalidOperationException("невозможно перемещать пустые ячейки");

      //first
      bool value = Match(first.Position, grid, swap, result);
      if (value && result == null) return true;
      //second
      value = Match(second.Position, grid, swap, result) || value;

      return value;
    }

    public virtual bool Match(Point tilePosition, ITileGridProvider grid, MatchCombinationsResult result = null)
    {
      return Match(tilePosition, grid, null, result);
    }

    protected virtual bool ItemsEquals(Tile pivot, Tile other)
    {
      return pivot.ItemType == ItemType.Cell && other.Item != null && other.Item.Id == pivot.Item.Id && (other.ItemType != ItemType.UniversalSwapCell && pivot.ItemType != ItemType.UniversalSwapCell);
    }

    protected int Priority
    {
      get { return _priority; }
    }

    private bool Match(Point position, ITileGridProvider grid, Swap swap, MatchCombinationsResult result)
    {
      var pivot = swap != null ? grid.GetTileBySwap(swap, position) : grid.GetTile(position);
      if (pivot == null) throw new NullReferenceException("невозможно производить поиск относительно несуществующей ячейке");

      var item = pivot.Item;
      if (item == null) throw new NullReferenceException("невозможно производить поиск комбинаций для пустой ячейки");

      foreach (var offsets in PatternOffsets)
      {
        var ok = true;
        foreach (var offset in offsets)
        {
          var tile = swap != null ? grid.GetTileBySwap(swap, position + offset) : grid.GetTile(position + offset);
          if (!(tile != null && ItemsEquals(pivot, tile)))
          {
            ok = false;
            break;
          }
        }
        if (ok)
        {
          if (result != null)
          {
            var positions = new Point[offsets.Length];
            for (int i = 0; i < positions.Length; i++)
            {
              positions[i] = position + offsets[i];
            }
            result.AddMatch(new Match(position, positions, GetGenerateItem(item), _priority));
          }
          return true;
        }
      }
      return false;
    }

    private Item GetGenerateItem(Item item)
    {
      if (_generateItem != null)
      {
        Item result;
        _generateItem.TryGetValue(item.Id, out result);
        return result;
      }
      return null;
    }
  }
}