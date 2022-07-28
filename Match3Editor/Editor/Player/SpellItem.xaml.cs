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

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для SpellItem.xaml
  /// </summary>
  public partial class SpellItem
  {
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(SpellItem), new PropertyMetadata(default(string)));
    private bool _selected;

    public SpellItem()
    {
      InitializeComponent();
    }

    public string Label
    {
      get { return (string)GetValue(LabelProperty); }
      set { SetValue(LabelProperty, value); }
    }

    public bool Selected
    {
      get { return _selected; }
      set
      {
        _selected = value;
        Background = value ? Brushes.Magenta : Brushes.White;
      }
    }

    public int Id { get; set; }
    public int Level { get; set; }
  }
}
