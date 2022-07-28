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
using Match3.Editor.LevelEditor;
using Match3.Engine.Levels;

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для TileModifier.xaml
  /// </summary>
  public partial class TileModifier : UserControl
  {
    private readonly List<LevelEditorModifierItem> _modifierItems = new List<LevelEditorModifierItem>();

    public TileModifier()
    {
      InitializeComponent();
    }

    public void Add(Modifier modifier)
    {
      var item = new LevelEditorModifierItem
      {
        Id = modifier.Id,
        Type = AppSettings.Setting.GetModifierType(modifier.Id),
        Count = modifier.Level
      };
      _modifierItems.Add(item);
      StackPanel.Children.Add(item);
    }

    public void Change(int modifierId, int modifierLevel)
    {
      var item = _modifierItems.Find(a => a.Id == modifierId);
      item.Count = modifierLevel;
      item.Visibility = modifierLevel > 0 ? Visibility.Visible : Visibility.Hidden;
    }
  }
}
