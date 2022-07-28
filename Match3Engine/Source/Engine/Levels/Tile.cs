using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Providers;
using System;
using System.Collections.Generic;

namespace Match3.Engine.Levels
{
  public class Tile
  {
    private readonly ITileGridProvider _grid;
    private readonly IEngineProviders _providers;
    private readonly int _objectId;
    private readonly Position _position;
    private readonly TileModifiers _modifiers;

    private Item _item;
    private Item _previousItem;
    private Item _nextItem;
    private bool _processed;
    private bool _nextItemInvalid;

    public Tile(ITileGridProvider grid, LevelTileDescription description, IEngineProviders providers, int objectId)
    {
      var modifiers = description.Modifiers;
      if (modifiers != null && modifiers.Length != 0)
      {
        modifiers = Array.ConvertAll(modifiers, input => input.Clone());
      }
      _modifiers = new TileModifiers(modifiers ?? new Modifier[0], providers);

      _grid = grid;
      _providers = providers;
      _objectId = objectId;
      _position = new Position(description.Position);

      Direction = description.Direction;
      Type = description.Type;

      SetItem(description.Item);
    }

    public TileType Type { get; private set; }
    public Direction Direction { get; private set; }

    public Position Position { get { return _position; } }

    public IEnumerable<Modifier> Modifiers { get { return _modifiers.Modifiers; } }

    public bool IsEmpty { get { return _item == null; } }
    public bool IsSwapable { get { return !IsEmpty && Type == TileType.Movable && _modifiers.CanSwap && _modifiers.CanReceiveItem; } }
    public bool IsMovable { get { return Type == TileType.Movable && _modifiers.CanMove && _modifiers.CanReceiveItem; } }
    public bool IsCanReceiveItem { get { return _modifiers.CanReceiveItem; } }
    public Item Item { get { return _item; } }
    public Item PreviousItem { get { return _previousItem; } }
    public Item NextItem { get { return _nextItem; } }
    public ItemType ItemType
    {
      get
      {
        return _item != null ? _providers.ItemsProvider.Get(_item.Id).Type : ItemType.Undefined;
      }
    }

    public bool IsProcessed { get { return _processed; } }

    public Tile LeftTile { get { return _grid.GetTile(_position, Direction.Left); } }
    public Tile RightTile { get { return _grid.GetTile(_position, Direction.Right); } }
    public Tile TopTile { get { return _grid.GetTile(_position, Direction.Top); } }
    public Tile BottomTile { get { return _grid.GetTile(_position, Direction.Bottom); } }

    /// <summary>
    /// следующая ячейка в которую может переместится по гравитации, если по направлению есть телепорт, следующая ячейка будет выход из телепорта
    /// </summary>
    public Fall Next
    {
      get
      {
        var edge = _grid.GetEdgeByTile(_position, Direction);
        if (edge.Type == EdgeType.TeleportInput && edge.Direction == Direction.Invert())
        {
          var outEdge = _grid.GetEdgeBy(edge.Index, EdgeType.TeleportOutput);
          var nextTeleportTile = _grid.GetTileByEdge(outEdge.Position, outEdge.Direction);
          if (nextTeleportTile != null)
          {
            return new Fall(nextTeleportTile, MoveType.Teleport, Direction, outEdge.Direction);
          }

          return Fall.Empty;
        }

        var nextTile = _grid.GetTile(_position, Direction);
        if (nextTile != null)
        {
          return new Fall(nextTile, MoveType.Fall, Direction, Direction.Invert());
        }
        return Fall.Empty;
      }
    }

    /// <summary>
    /// предыдущая ячейка с которой может переместится по гравитации, если по направлению есть телепорт, предыдущая ячейка будет вход в телепорта
    /// </summary>
    public Fall Prev
    {
      get
      {
        var edge = _grid.GetEdgeByTile(_position, Direction.Invert());
        if (edge.Type == EdgeType.TeleportOutput && edge.Direction == Direction)
        {
          var outEdge = _grid.GetEdgeBy(edge.Index, EdgeType.TeleportInput);
          var nextTeleportTile = _grid.GetTileByEdge(outEdge.Position, outEdge.Direction);
          if (nextTeleportTile != null)
          {
            return new Fall(nextTeleportTile, MoveType.Teleport, Direction.Invert(), outEdge.Direction);
          }
          return Fall.Empty;
        }

        var nextTime = _grid.GetTile(_position, Direction.Invert());
        if (nextTime != null)
        {
          return new Fall(nextTime, MoveType.Fall, Direction.Invert(), Direction);
        }
        return Fall.Empty;
      }
    }

    public Edge LeftEdge { get { return _grid.GetEdgeByTile(_position, Direction.Left); } }
    public Edge RightEdge { get { return _grid.GetEdgeByTile(_position, Direction.Right); } }
    public Edge TopEdge { get { return _grid.GetEdgeByTile(_position, Direction.Top); } }
    public Edge BottomEdge { get { return _grid.GetEdgeByTile(_position, Direction.Bottom); } }
    public Edge DirectionEdge { get { return _grid.GetEdgeByTile(_position, Direction); } }

    public int ObjectId { get { return _objectId; } }

    public void SetItem(Item item)
    {
      _previousItem = _item;

      _item = item;
    }

    public void SetNextItem(Item nextItem)
    {
      _nextItem = nextItem;
      _nextItemInvalid = true;
    }

    public void BeginProcess()
    {
      _processed = true;
    }

    public void EndProcess()
    {
      _processed = false;
    }

    public void ApplyNextItem()
    {
      if (_nextItemInvalid)
      {
        SetItem(_nextItem);
        _nextItem = null;
        _nextItemInvalid = false;
      }
    }

    public void ApplyCollect(ModifierActivatorType activatorType, List<Modifier> modifiers = null)
    {
      _modifiers.ApplyCollect(activatorType, modifiers);
    }

    public override string ToString()
    {
      return string.Format("Tile({0}|{1}|{2}|{3})", Position, Item, Type, Direction);
    }
  }
}
