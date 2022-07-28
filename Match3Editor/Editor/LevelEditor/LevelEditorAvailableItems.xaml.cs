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

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorAvailableItems.xaml
  /// </summary>
  public partial class LevelEditorAvailableItems : UserControl
  {
    private LevelEditorAvailableItem[] _availbaleItems;
    private bool _blockEvent;

    public event Action Changed;

    public LevelEditorAvailableItems()
    {
      InitializeComponent();
    }

    public void Load(int[] availableItems)
    {
      var list = new List<LevelEditorAvailableItem>();
      foreach (var item in availableItems)
      {
        var current = new LevelEditorAvailableItem();
        current.Checked += OnCheckBoxChanged;
        current.Unchecked += OnCheckBoxChanged;
        current.ItemId = item;
        list.Add(current);
        Items.Children.Add(current);
      }
      _availbaleItems = list.ToArray();
    }

    public void Available(int[] items)
    {
      _blockEvent = true;
      foreach (var element in _availbaleItems)
      {
        element.IsChecked = false;
      }

      foreach (var item in items)
      {
        foreach (var element in _availbaleItems)
        {
          if (element.ItemId == item)
          {
            element.IsChecked = true;
          }
        }
      }
      _blockEvent = false;
    }

    public int[] AvailableItems
    {
      get { return _availbaleItems.Where(i => (bool)i.IsChecked).Select(i => i.ItemId).ToArray(); }
    }

    private void OnCheckBoxChanged(object sender, RoutedEventArgs args)
    {
      if (_blockEvent) return;
      if (Changed != null)
      {
        if (AvailableItems.Length == 0)
        {
          MessageBox.Show("available items is empty");
          ((LevelEditorAvailableItem)sender).IsChecked = true;
        }
        else
        {
          Changed();
        }
      }
    }
  }
}
