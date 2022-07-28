using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using Match3.Editor.Utils;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;
using Orientation = Match3.Engine.Levels.Orientation;
using Point = Match3.Engine.Levels.Point;

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для EdgeControl.xaml
  /// </summary>
  public partial class EdgeControl : Button, INotifyPropertyChanged
  {
    private Brush _color;

    public Brush EdgeColor
    {
      get { return _color; }
      set
      {
        _color = value;
        OnPropertyChanged(nameof(EdgeColor));
      }
    }

    public Visibility IsVisibleIcon { get; set; }

    public Point Position { get; set; }

    public Direction Direction { get; set; }

    public EdgeType Type { get; set; }

    public byte Index { get; set; }

    public Orientation Orientation { get; set; }

    public PackIconKind DirectionIconKind
    {
      get { return DirectionToPackIcon.ToPackIcon(Direction); }
    }

    public EdgeControl()
    {
      _color = Brushes.AntiqueWhite;
      DataContext = this;
      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetContent(CreateEvent.EdgeInfo info)
    {
      Position = info.Position;
      Orientation = info.Orientation;
      Index = info.Index;
      Type = info.Type;
      Direction = info.Direction;

      IsVisibleIcon = Type != EdgeType.None ? Visibility.Visible : Visibility.Hidden;

      OnPropertyChanged(nameof(IsVisibleIcon));
      OnPropertyChanged(nameof(DirectionIconKind));

      EdgeColor = EdgeTypeToColor.ToColor(Type);

      if (info.Orientation == Orientation.Horizontal)
      {
        Button.Width = CoordinateConverter.EdgeSize;
        Button.Height = CoordinateConverter.TileSize - CoordinateConverter.EdgeSize;
      }
      else
      {
        Button.Width = CoordinateConverter.TileSize - CoordinateConverter.EdgeSize;
        Button.Height = CoordinateConverter.EdgeSize;
        Button.MinHeight = CoordinateConverter.EdgeSize;
      }
    }

    private void EdgeControl_OnClick(object sender, RoutedEventArgs e)
    {
      Debug.WriteLine(Position.ToString() + string.Format(" {0}:{1}", Canvas.GetLeft(this), Canvas.GetTop(this)));
    }
  }
}
