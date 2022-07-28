using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3Editor.Annotations;

namespace Match3.Editor.LevelEditor
{
  public enum ItemRequirementType
  {
    //[Description("Collect")]
    Collect,
    //[Description("Generate")]
    Generate
  }

  /// <summary>
  /// Логика взаимодействия для LevelEditorRequirementItem.xaml
  /// </summary>
  public partial class LevelEditorRequirementItem : UserControl, INotifyPropertyChanged
  {
    private int _value;
    private string _label;
    private string _itemIdLabel;
    private int _level;
    private ItemRequirementType _type;
    private int _itemId;
    public static readonly DependencyProperty ItemCheckedProperty = DependencyProperty.Register(nameof(ItemChecked), typeof(bool), typeof(LevelEditorRequirementItem), new PropertyMetadata(default(bool), PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs evt)
    {
      ((LevelEditorRequirementItem)dependencyObject).ItemChecked = (bool)evt.NewValue;
    }

    private bool _isChecked;

    public EnumValueConverter<ItemRequirementType> ItemRequirementTypeEnum { get; set; } = new EnumValueConverter<ItemRequirementType>();

    public LevelEditorRequirementItem()
    {
      InitializeComponent();
    }

    public ItemRequirementType Type
    {
      get { return _type; }
      set
      {
        _type = value;
        OnPropertyChanged(nameof(Type));
      }
    }

    public int Level
    {
      get { return _level; }
      set
      {
        _level = value;
        OnPropertyChanged(nameof(Level));
      }
    }

    public string Label
    {
      get { return _label; }
      set
      {
        _label = value;
        OnPropertyChanged(nameof(Label));
      }
    }

    public bool IsValid
    {
      get { return _value.ToString() == Input.Text.ToString(); }
    }

    public MaterialDesignThemes.Wpf.PackIconKind CheckIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Check; } }
    public MaterialDesignThemes.Wpf.PackIconKind AlertIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Alert; } }

    public int Value
    {
      get { return _value; }
      set
      {
        if (_value == value) return;


        var lastValue = _value;
        HistoryContext.Execute(() =>
        {
          _value = value;
          OnPropertyChanged(nameof(Value));
          OnPropertyChanged(nameof(IsValid));
        }, () =>
        {
          _value = lastValue;
          OnPropertyChanged(nameof(Value));
          OnPropertyChanged(nameof(IsValid));
        }, $"change requirement value: '{lastValue}'->'{value}'");

      }
    }

    public string ItemIdLabel
    {
      get { return _itemIdLabel; }
      set
      {
        _itemIdLabel = value;
        OnPropertyChanged(nameof(ItemIdLabel));
      }
    }

    public IHistoryExecuter HistoryContext { get; set; }

    public Brush IconColor
    {
      get { return ItemToColor.ToColor(ItemId); }
    }

    public int ItemId
    {
      get { return _itemId; }
      set
      {
        _itemId = value;
        OnPropertyChanged(nameof(IconColor));
      }
    }

    public bool ItemChecked
    {
      get { return (bool)GetValue(ItemCheckedProperty); }
      set
      {
        if (ItemChecked == value) return;
        var last = _isChecked;
        HistoryContext.Execute(() =>
        {
          _isChecked = value;
          SetValue(ItemCheckedProperty, _isChecked);
          OnPropertyChanged(nameof(ItemChecked));
        }, () =>
        {
          _isChecked = last;
          SetValue(ItemCheckedProperty, _isChecked);
          OnPropertyChanged(nameof(ItemChecked));
        }, $"requirement item: '{ItemId}' check to:'{last}'->'{value}'");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Input_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }

    private void Input_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      OnPropertyChanged(nameof(IsValid));
    }

    private void Button_Apply_OnClick(object sender, RoutedEventArgs e)
    {
      OnPropertyChanged(nameof(Value));
      OnPropertyChanged(nameof(IsValid));
    }
  }
}
