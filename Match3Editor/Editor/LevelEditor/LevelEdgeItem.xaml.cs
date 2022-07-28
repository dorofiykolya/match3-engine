using Match3.Editor.Utils;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3Editor.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Orientation = Match3.Engine.Levels.Orientation;
using Point = System.Windows.Point;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEdgeItem.xaml
  /// </summary>
  public partial class LevelEdgeItem : Button, INotifyPropertyChanged
  {
    private static readonly PositionConverter PositionConverter = new PositionConverter();

    public Orientation Orientation;

    private Point _canvasPosition;
    private EdgeType _type;
    private byte _index;
    private Direction _direction;

    public LevelEdgeItem()
    {
      InitializeComponent();
    }

    public int X { get; set; }
    public int Y { get; set; }

    public EdgeType Type
    {
      get { return _type; }
      set
      {
        _type = value;
        Update();
        OnPropertyChanged(nameof(BackColor));
      }
    }

    public byte Index
    {
      get { return _index; }
      set
      {
        _index = value;
        Update();
      }
    }

    public Direction Direction
    {
      get { return _direction; }
      set
      {
        _direction = value;
        Update();
      }
    }

    public Brush BackColor
    {
      get
      {
        return EdgeTypeToColor.ToColor(Type);
      }
    }

    public Match3.Engine.Levels.Point Position { get { return new Match3.Engine.Levels.Point(X, Y); } }

    public Point CanvasPosition
    {
      get { return _canvasPosition; }
      set
      {
        _canvasPosition = value;
        Canvas.SetLeft(this, value.X - 10);
        Canvas.SetTop(this, value.Y - 10);
      }
    }

    public Item[] Queue { get; set; }

    public void SetEdge(LevelEdgeDescription edge)
    {
      Orientation = PositionConverter.GetEdgeOrientationByPosition(edge.Position);
      Direction = edge.Direction;
      Index = edge.Index;
      Type = edge.Type;
      Queue = edge.Queue;
    }

    private void Update()
    {
      if (!Direction.IsValidDirection())
      {
        if (Orientation == Orientation.Horizontal) Direction = Direction.Left;
        else Direction = Direction.Bottom;
      }

      Icon.Kind = EdgeTypeToPackIcon.ToPackIcon(Type);
      DirectionArrow.Kind = DirectionToPackIcon.ToPackIcon(Direction);
      IndexLabel.Content = Index.ToString();

      ToolTip = $"Type: '{Type}';{Environment.NewLine} Direction: '{Direction}';{Environment.NewLine} Index: '{Index}';{Environment.NewLine} Orientation: '{Orientation}'";

      if (Orientation == Orientation.Horizontal)
      {
        StackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
        Content.Width = 20;
        Content.Height = CoordinateConverter.TileSize;
      }
      else
      {
        StackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
        Content.Width = CoordinateConverter.TileSize;
        Content.Height = 20;
      }

      if (Type == EdgeType.None)
      {
        Icon.Visibility = Visibility.Hidden;
        Rect.Visibility = Visibility.Hidden;
        DirectionArrow.Visibility = Visibility.Hidden;
        IndexLabel.Visibility = Visibility.Hidden;
      }
      else
      {
        Icon.Visibility = Visibility.Visible;
        Rect.Visibility = Visibility.Visible;
        DirectionArrow.Visibility = Type != EdgeType.Lock ? Visibility.Visible : Visibility.Hidden;
        if (Type == EdgeType.TeleportInput || Type == EdgeType.TeleportOutput)
        {
          IndexLabel.Visibility = Visibility.Visible;
        }
      }
    }

    private void MouseEnterHandler(object sender, RoutedEventArgs e)
    {
      Hover.Visibility = Visibility.Visible;
    }

    private void MouseLeaveHandler(object sender, RoutedEventArgs e)
    {
      Hover.Visibility = Visibility.Hidden;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
