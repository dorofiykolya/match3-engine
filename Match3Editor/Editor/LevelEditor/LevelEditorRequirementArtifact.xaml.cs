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
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorRequirementArtifact.xaml
  /// </summary>
  public partial class LevelEditorRequirementArtifact : UserControl
  {
    public static readonly DependencyProperty ButtonApplyIconProperty = DependencyProperty.Register(nameof(ButtonApplyIcon), typeof(PackIconKind), typeof(LevelEditorRequirementArtifact), new PropertyMetadata(default(MaterialDesignThemes.Wpf.PackIconKind)));
    public static readonly DependencyProperty ItemIdLabelProperty = DependencyProperty.Register(nameof(ItemIdLabel), typeof(string), typeof(LevelEditorRequirementArtifact), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty ItemCheckedProperty = DependencyProperty.Register(nameof(ItemChecked), typeof(bool), typeof(LevelEditorRequirementArtifact), new PropertyMetadata(default(bool), ItemCheckedPropertyChangedCallback));

    private static void ItemCheckedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs evt)
    {
      ((LevelEditorRequirementArtifact)dependencyObject).ItemChecked = (bool)evt.NewValue;
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(LevelEditorRequirementArtifact), new PropertyMetadata(default(int), ValuePropertyChangedCallback));

    private static void ValuePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs evt)
    {
      ((LevelEditorRequirementArtifact)dependencyObject).Value = (int)evt.NewValue;
    }

    private string _label;
    private int _level;
    private int _itemId;

    public Func<int, int, int> CalculateAction;

    private int _value;
    private bool _isChecked;
    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(LevelEditorRequirementArtifact), new PropertyMetadata(default(string)));

    public LevelEditorRequirementArtifact()
    {
      InitializeComponent();
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

    public void IsValid()
    {
      if (_value.ToString() == Input.Text)
      {
        ButtonApplyIcon = CheckIcon;
      }
      else
      {
        ButtonApplyIcon = AlertIcon;
      }
    }

    public MaterialDesignThemes.Wpf.PackIconKind CheckIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Check; } }
    public MaterialDesignThemes.Wpf.PackIconKind AlertIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Alert; } }

    public int Value
    {
      get { return (int)GetValue(ValueProperty); }
      set
      {
        if (_value == value) return;

        var lastValue = _value;
        HistoryContext.Execute(() =>
        {
          _value = value;
          SetValue(ValueProperty, _value);
          OnPropertyChanged(nameof(Value));
          OnPropertyChanged(nameof(IsValid));
        }, () =>
        {
          _value = lastValue;
          SetValue(ValueProperty, _value);
          OnPropertyChanged(nameof(Value));
          OnPropertyChanged(nameof(IsValid));
        }, $"change requirement value: '{lastValue}'->'{value}'");
      }
    }

    public string ItemIdLabel
    {
      get { return (string)GetValue(ItemIdLabelProperty); }
      set
      {
        SetValue(ItemIdLabelProperty, value);
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
        ItemIdLabel = value.ToString();
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

    public MaterialDesignThemes.Wpf.PackIconKind ButtonApplyIcon
    {
      get { return (MaterialDesignThemes.Wpf.PackIconKind)GetValue(ButtonApplyIconProperty); }
      set
      {
        SetValue(ButtonApplyIconProperty, value);
        OnPropertyChanged(nameof(ButtonApplyIcon));
      }
    }

    public string Type
    {
      get { return (string) GetValue(TypeProperty); }
      set { SetValue(TypeProperty, value); }
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
      IsValid();
    }

    private void Button_Apply_OnClick(object sender, RoutedEventArgs e)
    {
      OnPropertyChanged(nameof(Value));
      IsValid();
    }

    private void Button_Calculate_OnClick(object sender, RoutedEventArgs e)
    {
      if (CalculateAction != null)
      {
        Value = CalculateAction(_itemId, _level);
        Input.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
      }
    }
  }
}
