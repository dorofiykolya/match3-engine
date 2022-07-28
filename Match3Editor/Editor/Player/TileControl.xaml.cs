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
using Match3.Editor.Utils;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Point = Match3.Engine.Levels.Point;

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для TileControl.xaml
  /// </summary>
  public partial class TileControl : Canvas
  {
    private TileItem _item;
    private System.Windows.Point _canvasPosition;
    public static readonly DependencyProperty SwapVisibleProperty = DependencyProperty.Register("SwapVisible", typeof(bool), typeof(TileControl), new PropertyMetadata(default(bool)));

    public TileControl()
    {
      InitializeComponent();
    }

    public TileItem Item
    {
      get { return _item; }
      set { _item = value; }
    }

    public Point Position { get; set; }

    public bool Selected { get; private set; }

    public System.Windows.Point CanvasPosition
    {
      get { return _canvasPosition; }
      set
      {
        _canvasPosition = value;
        CanvasUtils.SetPosition(this, value);
      }
    }

    public bool SwapVisible
    {
      get { return (bool) GetValue(SwapVisibleProperty); }
      set { SetValue(SwapVisibleProperty, value); }
    }

    public void SetContent(CreateEvent.TileInfo info)
    {

    }

    public void Unselect()
    {
      Selected = false;
      if (Item != null)
      {
        Item.Unselect();
      }
    }

    public void Select()
    {
      if (Item != null)
      {
        Selected = true;
        Item.Select();
      }
      else
      {
        Unselect();
      }
    }

    public void AddModifiers(Modifier[] modifiers)
    {
      if (modifiers != null && modifiers.Length != 0)
      {
        foreach (var modifier in modifiers)
        {
          Modifier.Add(modifier);
        }
      }
    }
  }
}
