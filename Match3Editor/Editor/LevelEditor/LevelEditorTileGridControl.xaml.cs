using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Match3.Editor.Player;
using Match3.Editor.Utils;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Levels;
using Match3.Engine.Utils;
using Point = Match3.Engine.Levels.Point;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorTileGridControl.xaml
  /// </summary>
  public partial class LevelEditorTileGridControl : UserControl
  {
    private readonly Dictionary<Match3.Engine.Levels.Point, TileItem> _itemsMap = new Dictionary<Point, TileItem>();
    private readonly Dictionary<Match3.Engine.Levels.Point, LevelEditorModifierItem> _modifiersMap = new Dictionary<Point, LevelEditorModifierItem>();
    private readonly Dictionary<Match3.Engine.Levels.Point, LevelEditorItemTile> _tilesMap = new Dictionary<Point, LevelEditorItemTile>();
    private readonly Dictionary<Match3.Engine.Levels.Point, LevelEdgeItem> _edgesMap = new Dictionary<Point, LevelEdgeItem>();
    private readonly Dictionary<Match3.Engine.Levels.Point, LevelEditorDirectionItem> _directionsMap = new Dictionary<Point, LevelEditorDirectionItem>();

    public event Action<Match3.Engine.Levels.Point> PositionItem;
    private Match3.Engine.Levels.Point _lastPosition;

    public event Action<Match3.Engine.Levels.Point> EdgePositionItem;
    private Match3.Engine.Levels.Point _lastEdgePosition;
    private CoordinateConverter _converter;
    private PositionConverter _positionConverter;

    public LevelEditorTileGridControl()
    {
      InitializeComponent();
    }

    private void LevelEditorItemTile_Loaded(object sender, RoutedEventArgs e)
    {
      _lastPosition = new Match3.Engine.Levels.Point(-1, -1);
    }

    public LevelEditorDirectionItem GetDirection(Match3.Engine.Levels.Point position)
    {
      LevelEditorDirectionItem direction;
      if (_directionsMap.TryGetValue(position, out direction))
      {
        return direction;
      }
      return null;
    }

    public LevelEdgeItem GetEdge(Match3.Engine.Levels.Point position)
    {
      LevelEdgeItem edge;
      if (_edgesMap.TryGetValue(position, out edge))
      {
        return edge;
      }
      return null;
    }

    public LevelEditorItemTile GetTile(Match3.Engine.Levels.Point position)
    {
      LevelEditorItemTile tileItem;
      if (_tilesMap.TryGetValue(position, out tileItem))
      {
        return tileItem;
      }
      return null;
    }

    public int ItemCount(int id, int level)
    {
      return _itemsMap.Values.Count(i => i.Item.Id == id && i.Item.Level == level);
    }

    public TileItem GetItem(Match3.Engine.Levels.Point position)
    {
      TileItem tileItem;
      if (_itemsMap.TryGetValue(position, out tileItem))
      {
        return tileItem;
      }
      return null;
    }

    public void RemoveTile(Match3.Engine.Levels.Point position)
    {
      var tile = GetTile(position);
      if (tile != null)
      {
        _tilesMap.Remove(position);
      }
      Tiles.Children.Remove(tile);
      foreach (var direction in new[] { Direction.Left, Direction.Right, Direction.Top, Direction.Bottom })
      {
        var next = position + Point.Direction(direction);
        if (GetTile(next) == null)
        {
          var edgePosition = _positionConverter.TileToEdge(position, direction);
          LevelEdgeItem edge;
          if (_edgesMap.TryGetValue(edgePosition, out edge))
          {
            Edges.Children.Remove(edge);
            _edgesMap.Remove(edgePosition);
          }
        }
      }
      LevelEditorDirectionItem directionItem;
      if (_directionsMap.TryGetValue(position, out directionItem))
      {
        Directions.Children.Remove(directionItem);
        _directionsMap.Remove(position);
      }
    }

    public LevelEditorItemTile AddTile(Match3.Engine.Levels.Point position)
    {
      var tile = new LevelEditorItemTile
      {
        Position = position,
        CanvasPosition = _converter.ToCanvas(position, true)
      };
      _tilesMap[tile.Position] = tile;

      Tiles.Children.Add(tile);

      foreach (var direction in new[] { Direction.Left, Direction.Right, Direction.Top, Direction.Bottom })
      {
        LevelEdgeItem edge;
        var edgePosition = _positionConverter.TileToEdge(position, direction);
        if (!_edgesMap.TryGetValue(edgePosition, out edge))
        {
          edge = new LevelEdgeItem();
          edge.SetEdge(new LevelEdgeDescription
          {
            Direction = Direction.None,
            Type = EdgeType.None,
            Position = edgePosition,
            Index = 0,
            Queue = new Item[0]
          });
          edge.X = edgePosition.X;
          edge.Y = edgePosition.Y;
          edge.CanvasPosition = _converter.ToCanvasEdge(edgePosition, true);
          Edges.Children.Add(edge);
          _edgesMap[edgePosition] = edge;

          edge.Click += (sender, args) =>
          {
            var current = ((LevelEdgeItem)sender).Position;
            //if (current != _lastEdgePosition)
            {
              _lastEdgePosition = current;
              if (EdgePositionItem != null) EdgePositionItem(current);
            }
          };
        }
      }

      LevelEditorDirectionItem directionTile;
      if (!_directionsMap.TryGetValue(position, out directionTile))
      {
        directionTile = new LevelEditorDirectionItem();
        _directionsMap[tile.Position] = directionTile;
        CanvasUtils.SetPosition(directionTile, _converter.ToCanvas(tile.Position, true));
        directionTile.Direction = Direction.Bottom;
        Directions.Children.Add(directionTile);
      }

      return tile;
    }

    public void SetContent(LevelDescription grid)
    {
      _converter = new CoordinateConverter();
      _positionConverter = new PositionConverter();

      _itemsMap.Clear();
      _edgesMap.Clear();
      _directionsMap.Clear();
      _tilesMap.Clear();
      _modifiersMap.Clear();

      Modifiers.Children.Clear();
      Directions.Children.Clear();
      Edges.Children.Clear();
      Tiles.Children.Clear();
      Items.Children.Clear();

      foreach (var item in grid.Tiles)
      {
        AddTile(item.Position);
      }

      foreach (var edge in grid.Edges)
      {
        LevelEdgeItem edgeItem;
        if (!_edgesMap.TryGetValue(edge.Position, out edgeItem))
        {
          var item = new LevelEdgeItem();
          item.SetEdge(edge);
          item.X = edge.Position.X;
          item.Y = edge.Position.Y;
          item.CanvasPosition = _converter.ToCanvasEdge(edge.Position, true);
          item.Queue = edge.Queue;

          item.Click += (sender, args) =>
          {
            var current = ((LevelEdgeItem)sender).Position;
            //if (current != _lastEdgePosition)
            {
              _lastEdgePosition = current;
              if (EdgePositionItem != null) EdgePositionItem(current);
            }
          };

          Edges.Children.Add(item);
        }
        else
        {
          edgeItem.SetEdge(edge);
        }
      }

      foreach (var tile in grid.Tiles)
      {
        AddItem(tile);
      }

      foreach (var tile in grid.Tiles)
      {
        _directionsMap[tile.Position].Direction = tile.Direction;
        if (tile.Modifiers != null)
        {
          var tileItem = _tilesMap[tile.Position];
          foreach (var modifier in tile.Modifiers)
          {
            tileItem.AddModifier(modifier.Id, modifier.Level);
          }
        }
      }
    }

    public LevelTileDescription[] SerializeItems()
    {
      var list = new List<LevelTileDescription>();

      foreach (var tile in _tilesMap)
      {
        var tileData = new LevelTileDescription();
        tileData.Direction = _directionsMap[tile.Key].Direction;
        tileData.Type = TileType.Movable;
        tileData.Position = tile.Key;
        tileData.Modifiers = tile.Value.ToModifiers();

        TileItem item;
        if (_itemsMap.TryGetValue(tile.Key, out item))
        {
          tileData.Item = item.Item;
          tileData.Type = item.Type;
        }

        list.Add(tileData);
      }

      return list.ToArray();
    }

    public LevelEdgeDescription[] SerializeEdges()
    {
      var list = new List<LevelEdgeDescription>();
      foreach (var item in _edgesMap)
      {
        list.Add(new LevelEdgeDescription
        {
          Direction = item.Value.Direction,
          Type = item.Value.Type,
          Position = item.Key,
          Index = item.Value.Index,
          Queue = item.Value.Queue
        });
      }
      return list.ToArray();
    }

    public void ResetLastEdgePosition()
    {
      _lastEdgePosition = new Point(-1, -1);
    }

    public void ResetLastPosition()
    {
      _lastPosition = new Point(-1, -1);
    }

    public void RemoveItem(Point point)
    {
      TileItem item;
      if (_itemsMap.TryGetValue(point, out item))
      {
        _itemsMap.Remove(point);
        Items.Children.Remove(item);
      }
    }

    public void AddItem(LevelTileDescription tile)
    {
      if (GetTile(tile.Position) != null)
      {
        AddItem(tile.Position, tile.Item);
      }
    }

    public void AddItem(Point position, Item item)
    {
      if (GetTile(position) != null)
      {
        TileItem view;
        if (!_itemsMap.TryGetValue(position, out view))
        {
          view = new TileItem();
          _itemsMap.Add(position, view);
          CanvasUtils.SetPosition(view, _converter.ToCanvas(position, true));
          view.Position = position;
          Items.Children.Add(view);
        }
        view.SetContent(item);
      }
    }

    public void AddItem(TileItem tile)
    {
      if (GetTile(tile.Position) != null)
      {
        _itemsMap.Add(tile.Position, tile);
        Items.Children.Add(tile);
      }
    }

    public void Shuffle(out Action action, out Action undo)
    {
      var random = new EngineRandom(Environment.TickCount);
      var items = _itemsMap.ToList();
      var actionList = new List<Action>();
      var undoList = new List<Action>();
      while (items.Count > 0)
      {
        var current = items[items.Count - 1];
        var nextIndex = random.Next(items.Count - 1);
        var next = items[nextIndex];

        items.Remove(current);
        items.Remove(next);

        var currentItem = current.Value.Item;
        var nextItem = next.Value.Item;

        actionList.Add(() =>
        {
          current.Value.SetContent(nextItem);
          next.Value.SetContent(currentItem);
        });
        undoList.Add(() =>
        {
          current.Value.SetContent(currentItem);
          next.Value.SetContent(nextItem);
        });

      }
      action = () => actionList.ForEach(a => a());
      undo = () => undoList.ForEach(a => a());
    }

    public void Regenerate(out Action action, out Action undo, int[] availableItems)
    {
      var actionList = new List<Action>();
      var undoList = new List<Action>();
      var random = new EngineRandom(Environment.TickCount);

      foreach (var tile in _tilesMap.Values)
      {
        var position = tile.Position;
        var item = GetItem(position);
        var newItem = availableItems[random.Next(availableItems.Length)];

        if (item == null)
        {
          actionList.Add(() =>
          {
            AddItem(position, new Item(newItem, 0));
          });
          undoList.Add(() =>
          {
            RemoveTile(position);
          });
        }
        else
        {
          var lastItem = item.Item;
          actionList.Add(() =>
          {
            item.SetContent(new Item
            {
              Id = newItem
            });
          });
          undoList.Add(() =>
          {
            item.SetContent(lastItem);
          });
        }
      }

      action = () => actionList.ForEach(a => a());
      undo = () => undoList.ForEach(a => a());
    }

    public int ModifiersCount(int aid)
    {
      var total = 0;
      foreach (var tile in _tilesMap.Values)
      {
        total += tile.GetCount(aid);
      }
      return total;
    }

    public void InitializeTiles()
    {
      for (int x = 0; x < 10; x++)
      {
        for (int y = 0; y < 10; y++)
        {
          var position = new Point(x, y);
          var canvasPosition = _converter.ToCanvas(position, true);
          var tile = new LevelEditorTileClick();
          Canvas.SetLeft(tile, canvasPosition.X);
          Canvas.SetTop(tile, canvasPosition.Y);
          tile.MouseMove += (sender, args) =>
          {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
              var current = position;
              //if (current != _lastPosition)
              {
                _lastPosition = current;
                if (PositionItem != null) PositionItem(current);
              }
            }
          };
          tile.MouseDown += (sender, args) =>
          {
            var current = position;
            //if (current != _lastPosition)
            {
              _lastPosition = current;
              if (PositionItem != null) PositionItem(current);
            }
          };
          TileClick.Children.Add(tile);
        }
      }
    }
  }
}
