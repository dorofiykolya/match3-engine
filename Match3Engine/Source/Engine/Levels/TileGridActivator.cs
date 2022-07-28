using System;
using System.Collections.Generic;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Matches;
using Match3.Engine.Providers;

namespace Match3.Engine.Levels
{
  public class TileGridActivator
  {
    private readonly EngineState _engineState;
    private readonly TileGrid _tileGrid;
    private readonly IMatchesProvider _matchesProvider;
    private readonly IItemDescriptionProvider _itemsProvider;
    private readonly ICombinationActivatorsProvider _combinationActivatorsProvider;
    private readonly EngineEnvironment _environment;
    private readonly Dictionary<Point, int> _activatedMap;
    private readonly Dictionary<Point, Item> _generateItems;
    private readonly Context _context;
    private bool _inProcess;

    public TileGridActivator(EngineState engineState, TileGrid tileGrid, IEngineProviders providers, EngineEnvironment environment)
    {
      _activatedMap = new Dictionary<Point, int>();
      _generateItems = new Dictionary<Point, Item>();

      _engineState = engineState;
      _tileGrid = tileGrid;
      _matchesProvider = providers.MatchesProvider;
      _itemsProvider = providers.ItemsProvider;
      _combinationActivatorsProvider = providers.CombinationActivatorsProvider;
      _environment = environment;

      _context = new Context(this);
    }

    public void Begin()
    {
      _inProcess = true;
      _activatedMap.Clear();
      _generateItems.Clear();
    }

    public void End()
    {
      _inProcess = false;
      _activatedMap.Clear();
      _generateItems.Clear();
    }

    public IEnumerable<KeyValuePair<Point, int>> Activated
    {
      get { return _activatedMap; }
    }

    public IEnumerable<KeyValuePair<Point, Item>> ToGenerate
    {
      get { return _generateItems; }
    }

    public void Activate(Point position, ActivationResult result = null)
    {
      if (!_inProcess)
      {
        throw new InvalidOperationException("невозможно производить эту операцию вне контекста обработки тиков");
      }

      ActivateInternal(position, position, null, null, CombinationAction.Default, result);
    }

    public void Activate(IEnumerable<Match> matches, ActivationResult result = null)
    {
      if (!_inProcess)
      {
        throw new InvalidOperationException("невозможно производить эту операцию вне контекста обработки тиков");
      }

      if (matches == null) throw new ArgumentNullException("аргумент \"matches\" не может быть null");

      var combinations = _engineState.Pool.PopList<Match>();
      combinations.AddRange(matches);
      ActivateCombinations(combinations, null, CombinationAction.OnlyResult, result);
      ActivateCombinations(combinations, null, CombinationAction.DoNotAddToResult, result);
      _engineState.Pool.PushList(combinations);
    }

    public void Activate(Swap swap, out MatchCombinationsResult combinationsResult, ActivationResult result = null)
    {
      if (!_inProcess)
      {
        throw new InvalidOperationException("невозможно производить эту операцию вне контекста обработки тиков");
      }

      combinationsResult = new MatchCombinationsResult();
      if (!_matchesProvider.Match(swap, _tileGrid, combinationsResult) || !combinationsResult.HasMatches)
      {
        throw new InvalidOperationException("операция невозможна, нельзя перемещать данные ячейки, комбинации не найдены: " + swap);
      }

      var fromItem = _tileGrid.GetTile(swap.First);
      var toItem = _tileGrid.GetTile(swap.Second);

      if (fromItem.IsEmpty || toItem.IsEmpty) throw new ArgumentException("ячейки не могут быть пустыми");
      if (!fromItem.IsMovable || !toItem.IsMovable) throw new ArgumentException("ячейки не могут быть не перемещаемыми");

      _tileGrid.Swap(swap);

      var itemActivator = _combinationActivatorsProvider.GetSwapActivator(fromItem.Item, toItem.Item);
      if (itemActivator != null)
      {
        itemActivator.ActivateSwap(_engineState, _context, fromItem, toItem, combinationsResult.Combinations, result);
      }
      else
      {
        var combinations = combinationsResult.Combinations;
        if (combinations.Count > 0 && combinations.Count <= 2)
        {
          ActivateCombinations(combinations, swap, CombinationAction.OnlyResult, result);
          ActivateCombinations(combinations, swap, CombinationAction.DoNotAddToResult, result);
        }
        else
        {
          throw new InvalidOperationException("при перемещении нашлось больше 2 комбинаций, чего не может быть: " + swap);
        }
      }
    }

    private void ActivateCombinations(IList<Match> combinations, Swap swap, CombinationAction combination, ActivationResult result = null)
    {
      foreach (var match in combinations)
      {
        var activated = true; //!ActivateInternal(match.Pivot, match.Pivot, swap, match.GenerateItem, combination, result);
        foreach (var position in match.Combination)
        {
          activated &= !ActivateInternal(position, match.Pivot, swap, match.GenerateItem, combination, result);
        }

        if (activated && match.GenerateItem != null && combination != CombinationAction.OnlyResult)
        {
          Generate(match.GenerateItem, match.Pivot, result);
        }
      }
    }

    private bool ActivateInternal(Point position, Point pivot, Swap swap, Item generateItem, CombinationAction combination = CombinationAction.Default, ActivationResult result = null)
    {
      var tile = _tileGrid.GetTile(position);
      if (tile != null && !tile.IsEmpty && (tile.ItemType == ItemType.Cell || tile.ItemType == ItemType.UniversalSwapCell))
      {
        if (combination == CombinationAction.OnlyResult)
        {
          if (result != null)
          {
            result.AddActivated(tile.Position, tile.Item, generateItem, pivot);
          }
          return false;
        }
        var isMapped = MapPosition(position);

        if (!isMapped || combination == CombinationAction.DoNotAddToResult)
        {
          if (result != null && combination != CombinationAction.DoNotAddToResult)
          {
            result.AddActivated(tile.Position, tile.Item, generateItem, pivot);
          }

          var activator = _combinationActivatorsProvider.GetActivator(tile.Item);
          if (activator != null)
          {
            var initiator = _tileGrid.GetTile(pivot);
            if (initiator == null) throw new InvalidOperationException("невозможно произвести активацию относительно не существующей точки");
            activator.Activate(_engineState, _context, tile, initiator, swap, result);
          }
          else
          {
            var itemDescription = _itemsProvider.Get(tile.Item.Id);
            var itemLevelDescription = itemDescription.GetLevel(tile.Item.Level);
            var itemActivator = itemLevelDescription.Activator;
            if (itemActivator != null && itemActivator.Offsets != null)
            {
              foreach (var offset in itemActivator.Offsets)
              {
                ActivateInternal(position + offset, position, swap, null, CombinationAction.Default, result);
              }
            }
          }
        }

        return isMapped;
      }

      return false;
    }

    private bool IsMappedPosition(Point position)
    {
      int activated;
      if (_activatedMap.TryGetValue(position, out activated))
      {
        return activated > 0;
      }

      return false;
    }

    private bool MapPosition(Point position)
    {
      int activated;
      if (_activatedMap.TryGetValue(position, out activated))
      {
        ++activated;
        _activatedMap[position] = activated;
        return true;
      }

      _activatedMap[position] = 1;
      return false;
    }

    private void Generate(Item item, Point pivot, ActivationResult result)
    {
      if (!_inProcess)
      {
        throw new InvalidOperationException("невозможно производить эту операцию вне контекста обработки тиков");
      }
      Item generateItem;
      if (_generateItems.TryGetValue(pivot, out generateItem))
      {
        throw new InvalidOperationException(string.Format("в данной ячейке уже производится генерация нового предмета, позиция: \"{0}\", уже в списке: \"{1}\", хотите добавить: \"{2}\"", pivot, generateItem, item));
      }
      _generateItems[pivot] = item;

      if (result != null)
      {
        result.AddGenerated(pivot, item);
      }
    }

    private enum CombinationAction
    {
      Default,
      OnlyResult,
      DoNotAddToResult
    }

    private class Context : IActivatorContext
    {
      private readonly TileGridActivator _activator;

      public Context(TileGridActivator activator)
      {
        _activator = activator;
      }

      public void Generate(Item item, Point pivot, ActivationResult result)
      {
        _activator.Generate(item, pivot, result);
      }

      public void Activate(Point position, Point pivot, Swap swap, ActivationResult result)
      {
        _activator.ActivateInternal(position, pivot, swap, null, CombinationAction.Default, result);
      }

      public bool CheckPosition(Point position)
      {
        return _activator.MapPosition(position);
      }
    }
  }
}
