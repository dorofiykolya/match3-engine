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
using Match3.Engine.Descriptions.Modifiers;
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorModifierItem.xaml
  /// </summary>
  public partial class LevelEditorModifierItem : UserControl, INotifyPropertyChanged
  {
    private int _count = 0;
    private PackIconKind _icon;
    private ModifierType _type;
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(LevelEditorModifierItem), new PropertyMetadata(default(Brush)));
    //Crosshairs BlockHelper

    public LevelEditorModifierItem()
    {
      InitializeComponent();
    }

    public int Count
    {
      get { return _count; }
      set
      {
        _count = value;
        OnPropertyChanged(nameof(Count));
      }
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

    public Brush Color
    {
      get { return (Brush)GetValue(ColorProperty); }
      set
      {
        SetValue(ColorProperty, value);
        OnPropertyChanged(nameof(Color));
      }
    }

    public ModifierType Type
    {
      get { return _type; }
      set
      {
        _type = value;
        Color = ModifierTypeToColor.ToColor(value);
      }
    }

    public int Id { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
