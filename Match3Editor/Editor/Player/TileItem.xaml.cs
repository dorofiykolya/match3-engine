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
using Match3.Editor.Utils;
using Match3.Engine.Levels;
using Match3Editor.Annotations;
using Point = System.Windows.Point;

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для TileItem.xaml
  /// </summary>
  public partial class TileItem : Canvas, INotifyPropertyChanged
  {
    private Brush _color;
    private string _level;
    private double _scale = 1.0;
    private Visibility _selected = Visibility.Hidden;
    public TileControl OwnerTile;
    private Point _canvasPosition;

    public double Scale
    {
      get { return _scale; }
      set
      {
        _scale = value;
        OnPropertyChanged(nameof(Scale));
      }
    }

    public Visibility Selected
    {
      get { return _selected; }
      set
      {
        _selected = value;
        OnPropertyChanged(nameof(Selected));
      }
    }

    public Point CanvasPosition
    {
      get { return _canvasPosition; }
      set
      {
        _canvasPosition = value;
        CanvasUtils.SetPosition(this, value);
      }
    }

    public string ItemLevel
    {
      get { return _level; }
      set
      {
        if (_level != value)
        {
          _level = value;
          OnPropertyChanged(nameof(ItemLevel));
        }
      }
    }

    public Brush Color
    {
      get { return _color; }
      set
      {
        if (_color != value)
        {
          _color = value;
          OnPropertyChanged(nameof(Color));
        }
      }
    }

    public Item Item { get; set; }
    public TileType Type { get; set; } = TileType.Movable;
    public Engine.Levels.Point Position { get; set; }

    public TileItem()
    {
      DataContext = this;
      InitializeComponent();

      Color = Brushes.Aqua;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetContent(Item item)
    {
      Item = item;

      if (item != null)
      {
        Color = ItemToColor.ToColor(item.Id);
        ItemLevel = (item.Level + 1).ToString();
      }
      else
      {
        Color = Brushes.Transparent;
        ItemLevel = "";
      }
    }

    public void Select()
    {
      Selected = Visibility.Visible;
    }

    public void Unselect()
    {
      Selected = Visibility.Hidden;
    }
  }
}
