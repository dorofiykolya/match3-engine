using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Utils;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// Амулет – «Цепная молния». Количество камней, который разбиваются по цепочке (игрок выбирает начальный, молния прыгает на соседние камни или через один)
  /// </summary>
  public class ChainDestroySpellTypeAction : SpellTypeAction
  {
    private readonly Point[] _offsets;
    private readonly Point[] _alternativeOffsets;

    public ChainDestroySpellTypeAction()
    {
      _offsets = PatternParser.Parse(@"[#|#|#]" +
                                      "[#|X|#]" +
                                      "[#|#|#]", true);

      _alternativeOffsets = PatternParser.Parse(@"[#|#|#|#|#]" +
                                                 "[#| | | |#]" +
                                                 "[#| |X| |#]" +
                                                 "[#| | | |#]" +
                                                 "[#|#|#|#|#]", true);
    }

    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var providers = state.Configuration.Providers;
      var spell = providers.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);
      var count = spellLevel.Value;
      var activator = state.TileGridActivator;

      var pivot = state.TileGrid.GetTile(useSpell.Positions[0]);
      if (pivot == null || pivot.IsEmpty) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + string.Format("невозможно использовать спелл, не верно заданы координаты ячейки, ячейка должна существовать и не может быть пуста, Spell(id:{0}, level:{1}, position:{2})", useSpell.Id, useSpell.Level, useSpell.Positions[0]));

      var offsets = _offsets.ToArray();
      var offsetCount = offsets.Length;
      while (--offsetCount > 0)
      {
        var index = state.GetNextRandom(offsetCount);
        var current = offsets[offsetCount];
        var next = offsets[index];
        offsets[offsetCount] = next;
        offsets[index] = current;
      }

      var alternativeOffsets = _alternativeOffsets.ToArray();
      offsetCount = alternativeOffsets.Length;
      while (--offsetCount > 0)
      {
        var index = state.GetNextRandom(offsetCount);
        var current = alternativeOffsets[offsetCount];
        var next = alternativeOffsets[index];
        alternativeOffsets[offsetCount] = next;
        alternativeOffsets[index] = current;
      }

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellEvt = null;
      if (state.Configuration.Environment.IsGenerateOutputEvents())
      {
        activationResult = new ActivationResult();
        useSpellEvt = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellEvt.UseSpell = useSpell;
      }

      var chain = FindChain(pivot, state.TileGrid, count + 1, offsets, alternativeOffsets);

      for (int i = 0; i < chain.Count; i++)
      {
        if (useSpellEvt != null)
        {
          useSpellEvt.ActivateTiles.Add(chain[i]);
        }
        activator.Activate(chain[i], activationResult);
      }

      if (activationResult != null)
      {
        var evt = state.Output.EnqueueByFactory<ActivateEvent>(state.Tick);
        evt.InitializeFrom(activationResult.Queue);
      }
    }

    /// <summary>
    /// поиск доступных цепочек
    /// </summary>
    /// <param name="pivot">точка относительно которой строится цепочка</param>
    /// <param name="grid"></param>
    /// <param name="max">длина цепочки</param>
    /// <param name="points"></param>
    /// <param name="offsets"></param>
    /// <returns>возвращает самую длинную цепочку</returns>
    public List<Point> FindChain(Tile pivot, TileGrid grid, int max, Point[] offsets, Point[] alternativeOffsets)
    {
      var result = new List<Point>();

      var root = new Chain(pivot.Position, null);
      FindChain(root, grid, pivot.Item.Id, offsets, alternativeOffsets, 0, max);
      var collection = root.Last.ToRoot();
      collection.Reverse();
      foreach (var chain in collection)
      {
        if (max <= 0) break;
        result.Add(chain.Position);
        --max;
      }

      return result;
    }

    private void FindChain(Chain chain, TileGrid grid, int itemId, Point[] offsets, Point[] alternativeOffsets, int index, int max)
    {
      if (index < max)
      {
        var position = chain.Position;
        foreach (var offset in offsets)
        {
          if (chain.Root.IsBreak) return;
          var tile = grid.GetTile(position + offset);
          if (tile != null && !tile.IsEmpty)
          {
            if (tile.Item.Id == itemId)
            {
              if (!chain.IsAncestor(tile.Position))
              {
                FindChain(new Chain(tile.Position, chain), grid, itemId, offsets, alternativeOffsets, index + 1, max);
              }
            }
          }
        }
        foreach (var offset in alternativeOffsets)
        {
          if (chain.Root.IsBreak) return;
          var tile = grid.GetTile(position + offset);
          if (tile != null && !tile.IsEmpty)
          {
            if (tile.Item.Id == itemId)
            {
              if (!chain.IsAncestor(tile.Position))
              {
                FindChain(new Chain(tile.Position, chain), grid, itemId, offsets, alternativeOffsets, index + 1, max);
              }
            }
          }
        }
      }
      else
      {
        chain.Root.Break();
      }
    }

    private class ChainEqualityComparer : IEqualityComparer<Chain>
    {
      public static readonly ChainEqualityComparer Default = new ChainEqualityComparer();

      public bool Equals(Chain x, Chain y)
      {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(null, y)) return false;
        if (ReferenceEquals(null, x)) return false;
        return x.Position == y.Position;
      }

      public int GetHashCode(Chain obj)
      {
        return obj.GetHashCode();
      }
    }

    private class Chain : IEquatable<Chain>
    {
      private List<Chain> _children;
      private Point _position;

      public Chain(Point position, Chain parent)
      {
        _position = position;
        Parent = parent;

        if (parent != null)
        {
          if (parent._children == null)
          {
            parent._children = new List<Chain>();
          }
          parent._children.Add(this);
        }
      }

      public bool IsRoot
      {
        get { return Parent == null; }
      }

      public Chain Root
      {
        get { return Parent == null ? this : Parent.Root; }
      }

      public List<Chain> ToRoot()
      {
        var result = new List<Chain>();
        var current = this;
        while (current != null)
        {
          result.Add(current);
          current = current.Parent;
        }
        return result;
      }

      public Chain Last
      {
        get
        {
          var depth = Depth;
          var result = this;
          if (_children != null)
          {
            foreach (var chain in _children)
            {
              var last = chain.Last;
              if (last.Depth > depth) result = last;
            }
          }
          return result;
        }
      }

      public bool IsAncestor(Point position)
      {
        if (_position == position) return true;
        var current = Parent;
        while (current != null)
        {
          if (current.Position == position) return true;
          current = current.Parent;
        }
        return false;
      }

      public bool IsBreak { get; private set; }

      public int Depth
      {
        get
        {
          var depth = 0;
          var current = Parent;
          while (current != null)
          {
            current = current.Parent;
            ++depth;
          }
          return depth;
        }
      }

      public Point Position { get { return _position; } }
      public Chain Parent { get; private set; }

      public bool Equals(Chain other)
      {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Position.Equals(other.Position);
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Chain)obj);
      }

      public override int GetHashCode()
      {
        return _position.GetHashCode();
      }

      private bool ChildrenContains(Point position)
      {
        if (_children != null)
        {
          foreach (var chain in _children)
          {
            if (chain.Position == position) return true;
          }
        }
        return false;
      }

      public void Break()
      {
        IsBreak = true;
      }
    }
  }
}
