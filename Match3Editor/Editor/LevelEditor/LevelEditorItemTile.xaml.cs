using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorItemTile.xaml
  /// </summary>
  public partial class LevelEditorItemTile : UserControl
  {
    private readonly Dictionary<int, LevelEditorModifierItem> _modifierItems = new Dictionary<int, LevelEditorModifierItem>();
    private Point _canvasPosition;

    public LevelEditorItemTile()
    {
      InitializeComponent();
    }

    public Match3.Engine.Levels.Point Position { get; set; }

    public Point CanvasPosition
    {
      get { return _canvasPosition; }
      set
      {
        _canvasPosition = value;
        Canvas.SetLeft(this, value.X);
        Canvas.SetTop(this, value.Y);
      }
    }

    public void AddModifier(LevelEditorModifierItem modifier)
    {
      _modifierItems[modifier.Id] = modifier;
      StackPanel.Children.Add(modifier);
    }

    public LevelEditorModifierItem[] Modifiers
    {
      get { return _modifierItems.Values.ToArray(); }
    }

    public LevelEditorModifierItem Get(int id)
    {
      LevelEditorModifierItem modifier;
      _modifierItems.TryGetValue(id, out modifier);
      return modifier;
    }

    public void RemoveAllModifiers()
    {
      foreach (var value in _modifierItems.Values)
      {
        StackPanel.Children.Remove(value);
      }
      _modifierItems.Clear();
    }

    public void AddModifier(int modifierId, int count = 1)
    {
      var item = Get(modifierId);
      if (item == null)
      {
        item = new LevelEditorModifierItem
        {
          Id = modifierId,
          Type = AppSettings.Setting.GetModifierType(modifierId),
        };
        _modifierItems.Add(modifierId, item);
        StackPanel.Children.Add(item);
      }
      item.Count += count;
    }

    public void RemoveModifier(int modifierId)
    {
      var item = Get(modifierId);
      if (item != null)
      {
        --item.Count;
        if (item.Count <= 0)
        {
          StackPanel.Children.Remove(item);
          _modifierItems.Remove(modifierId);
        }
      }
    }

    public Modifier[] ToModifiers()
    {
      var result = new List<Modifier>();
      foreach (var value in _modifierItems.Values)
      {
        result.Add(new Modifier
        {
          Id = value.Id,
          Level = value.Count
        });
      }
      return result.ToArray();
    }

    public int GetCount(int id)
    {
      var total = 0;
      foreach (var value in _modifierItems.Values)
      {
        if (value.Id == id)
        {
          total += value.Count;
        }
      }
      return total;
    }
  }
}
