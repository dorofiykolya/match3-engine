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
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorModifierToolbarItem.xaml
  /// </summary>
  public partial class LevelEditorModifierToolbarItem : INotifyPropertyChanged
  {
    public enum ActionType
    {
      Add,
      Remove
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(LevelEditorModifierToolbarItem), new PropertyMetadata("Armor"));
    private ActionType _type;
    private PackIconKind _icon;

    public LevelEditorModifierToolbarItem()
    {
      InitializeComponent();
    }

    public PackIconKind Icon
    {
      get { return _icon; }
      set
      {
        _icon = value;
        OnPropertyChanged(nameof(Icon));
      }
    }

    public ActionType Type
    {
      get { return _type; }
      set
      {
        _type = value;
        Icon = value == ActionType.Add ? PackIconKind.PlusCircleOutline : PackIconKind.MinusCircleOutline;
      }
    }

    public int Id { get; set; }

    public string Label
    {
      get { return (string)GetValue(LabelProperty); }
      set
      {
        SetValue(LabelProperty, value);
        OnPropertyChanged(nameof(Label));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
