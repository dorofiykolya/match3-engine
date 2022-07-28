using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Match3.Editor.Utils;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Point = Match3.Engine.Levels.Point;

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для TileGridControl.xaml
  /// </summary>
  public partial class TileGridControl : Canvas
  {
    private TileControl _selected;
    private CoordinateConverter _converter;
    private PlayerEngine _engine;
    private readonly Dictionary<Point, TileControl> _tiles = new Dictionary<Point, TileControl>();
    private readonly Dictionary<Point, EdgeControl> _edges = new Dictionary<Point, EdgeControl>();

    public TileGridControl()
    {
      InitializeComponent();
    }

    public bool AllowSwap { get { return _engine.AllowSwap; } }
    public bool IsUseSpell { get { return _engine.IsUseSpell; } }

    public void InitializeByEngine(PlayerEngine engine, int width, int height)
    {
      _engine = engine;
      _converter = new CoordinateConverter();
    }

    public TileControl[] Tiles { get { return _tiles.Values.ToArray(); } }

    public TileControl GetTile(Point position)
    {
      TileControl tile;
      _tiles.TryGetValue(position, out tile);
      return tile;
    }

    public void AddTile(CreateEvent.TileInfo info)
    {
      var position = info.Position;
      var tile = new TileControl();
      tile.MouseLeftButtonDown += TileOnMouseLeftButtonDown;
      tile.SetContent(info);
      tile.Position = position;
      tile.AddModifiers(info.Modifies);
      _tiles[position] = tile;

      if (info.Item != null)
      {
        AddItem(tile, info.Item);
      }

      TileCanvas.Children.Add(tile);

      var pos = _converter.ToCanvas(position);
      tile.CanvasPosition = pos;
    }

    public void Unselect()
    {
      if (_selected != null)
      {
        _selected.Unselect();
        _selected = null;
      }
    }

    private void TileOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
      if (!AllowSwap) return;
      if (IsUseSpell)
      {
        _engine.UseSpell(((TileControl)sender).Position);
        return;
      }

      if (_selected != null)
      {
        if (Swap.IsValid(((TileControl)sender).Position, _selected.Position))
        {
          var swap = new Swap(((TileControl)sender).Position, _selected.Position);
          if (_engine.CanSwap(swap))
          {
            _engine.Swap(swap);
            _selected.Unselect();
            _selected = null;
            return;
          }
        }

        _selected.Unselect();
        if (_selected == sender)
        {
          _selected = null;
        }
        else
        {
          _selected = (TileControl)sender;
          _selected.Select();
        }
      }
      else
      {
        _selected = (TileControl)sender;
        _selected.Select();
      }
    }

    public void AddItem(TileControl tile, Item item)
    {
      var itemControl = new TileItem();
      itemControl.OwnerTile = tile;
      tile.Item = itemControl;
      itemControl.CanvasPosition = _converter.ToCanvas(tile.Position);
      itemControl.SetContent(item);
      ItemCanvas.Children.Add(itemControl);
    }

    public void RemoveItem(Point position)
    {
      var tile = GetTile(position);
      var item = tile.Item;
      tile.Item = null;
      ItemCanvas.Children.Remove(item);
    }

    public EdgeControl GetEdge(Point position)
    {
      EdgeControl edge;
      _edges.TryGetValue(position, out edge);
      return edge;
    }

    public void AddEdge(CreateEvent.EdgeInfo info)
    {
      var position = info.Position;
      var edge = new EdgeControl();
      edge.SetContent(info);
      EdgeCanvas.Children.Add(edge);
      edge.Position = position;
      _edges[position] = edge;
      var pos = _converter.ToCanvasEdge(position);
      CanvasUtils.SetPosition(edge, pos);
    }

    public void RemoveAllTiles()
    {
      foreach (var tileControl in _tiles)
      {
        RemoveItem(tileControl.Key);
        TileCanvas.Children.Remove(tileControl.Value);
      }
      _tiles.Clear();
    }

    public void RemoveAllEdges()
    {
      foreach (var edgeControl in _edges)
      {
        EdgeCanvas.Children.Remove(edgeControl.Value);
      }
      _edges.Clear();
    }
  }
}
