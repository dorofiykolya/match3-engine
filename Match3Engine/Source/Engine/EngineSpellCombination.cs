using System;
using System.Collections.Generic;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.SpellCombinations;
using Match3.Engine.Levels;
using Match3.Engine.Matches;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  public class EngineSpellCombination
  {
    private readonly EngineState _engineState;
    private readonly SpellCombinationDescription[] _collection;
    private readonly ISpellDescriptionProvider _spellDescriptionProvider;
    private readonly EngineEnvironment _environment;
    private readonly Node _queueMap;
    private readonly Queue<int> _queue;

    public EngineSpellCombination(EngineState engineState, SpellCombinationDescription[] collection, ISpellDescriptionProvider spellDescriptionProvider, EngineEnvironment environment)
    {
      _engineState = engineState;
      _collection = collection;
      _spellDescriptionProvider = spellDescriptionProvider;
      _environment = environment;

      _queueMap = new Node(_collection);
      _queue = new Queue<int>();
    }

    public void Prune()
    {
      _queue.Clear();
    }

    public List<int> ItemQueueCombinations(List<int> result = null)
    {
      if (result == null) result = new List<int>();
      FillItemsQueue(result, _queue);
      return result;
    }

    public List<SpellCombinationDescription> PossibleCombinations(List<SpellCombinationDescription> result = null)
    {
      if (result == null) result = new List<SpellCombinationDescription>();
      FillPossible(result, _queue);
      return result;
    }

    public List<SpellCombinationDescription> AvailableCombinationSpells(List<SpellCombinationDescription> result = null)
    {
      if (result == null) result = new List<SpellCombinationDescription>();
      FillAvailable(result, _queue);
      return result;
    }

    public void Enqueue(Swap swap, MatchCombinationsResult matches, ITileGridProvider gridProvider)
    {
      if (!matches.HasMatches || matches.Combinations.Count > 2) throw new InvalidOperationException();
      var combinations = matches.Combinations;
      if (combinations.Count == 1)
      {
        var tile = gridProvider.GetTile(combinations[0].Pivot);
        if (tile.IsEmpty) throw new InvalidOperationException("ячейка не может быть пуста");
        _queue.Enqueue(tile.Item.Id);
        ProcessQueue();
      }
      else if (combinations.Count == 2)
      {
        var tile = gridProvider.GetTile(swap.First);
        if (tile.IsEmpty) throw new InvalidOperationException("ячейка не может быть пуста");
        if (tile.ItemType == ItemType.UniversalSwapCell)
        {
          tile = gridProvider.GetTile(swap.Second);
          if (tile.IsEmpty) throw new InvalidOperationException("ячейка не может быть пуста");
          if (tile.ItemType == ItemType.UniversalSwapCell)
          {
            return;
          }
        }
        if (tile.ItemType != ItemType.Cell) throw new InvalidOperationException("невозможно определить символ");
        _queue.Enqueue(tile.Item.Id);
        ProcessQueue();
      }
    }

    private void ProcessQueue()
    {
      var pool = _engineState.Pool.PopList<SpellCombinationDescription>();
      while (_queue.Count != 0 && PossibleCombinations(pool).Count == 0)
      {
        _queue.Dequeue();
        pool.Clear();
      }
      _engineState.Pool.PushList(pool);
    }

    private void FillItemsQueue(List<int> result, Queue<int> queue)
    {
      result.AddRange(queue);
    }

    private void FillAvailable(List<SpellCombinationDescription> result, Queue<int> queue)
    {
      if (queue.Count != 0)
      {
        var node = _queueMap;
        foreach (var itemId in queue)
        {
          node = node[itemId];
          if (node == null) return;
        }
        if (node.Spell != null)
        {
          result.Add(node.Spell);
        }
      }
    }

    private void FillPossible(List<SpellCombinationDescription> result, Queue<int> queue)
    {
      if (queue.Count != 0)
      {
        var node = _queueMap;
        foreach (var itemId in queue)
        {
          node = node[itemId];
          if (node == null) return;
        }
        node.PossibleCombinations(result);
      }
    }

    private class Node
    {
      private readonly Node _parent;
      private readonly Dictionary<int, Node> _queue;

      public SpellCombinationDescription Spell;
      public int MaxDepth { get; private set; }

      public Node(SpellCombinationDescription[] collection) : this()
      {
        foreach (var description in collection)
        {
          var node = this;
          var depth = 0;

          foreach (var itemId in description.CombinationQueue)
          {
            node = node.AddNode(itemId);
            ++depth;
          }
          if (MaxDepth < depth) MaxDepth = depth;
          node.Spell = description;
        }
      }

      private Node()
      {
        _queue = new Dictionary<int, Node>();
      }

      private Node(Node parent = null) : this()
      {
        _parent = parent;
      }

      private Node AddNode(int itemId)
      {
        Node node;
        if (!_queue.TryGetValue(itemId, out node))
        {
          _queue[itemId] = node = new Node(this);
        }
        return node;
      }

      public Node this[int itemId]
      {
        get
        {
          Node node;
          _queue.TryGetValue(itemId, out node);
          return node;
        }
      }

      public void PossibleCombinations(List<SpellCombinationDescription> result)
      {
        if (Spell != null)
        {
          result.Add(Spell);
        }
        foreach (var node in _queue.Values)
        {
          node.PossibleCombinations(result);
        }
      }
    }
  }
}
