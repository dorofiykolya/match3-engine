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

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorAvailableItem.xaml
  /// </summary>
  public partial class LevelEditorAvailableItem : CheckBox
  {
    private int _itemId;

    public int ItemId
    {
      get { return _itemId; }
      set
      {
        _itemId = value;
        BackgroundColor.Fill = ItemToColor.ToColor(value);
        Label.Content = ItemId.ToString();
      }
    }

    public LevelEditorAvailableItem()
    {
      InitializeComponent();
    }
  }
}
