using System;
using System.Collections.Generic;
using System.Linq;
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
using Match3.Engine.Descriptions.Modifiers;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorRequirementModifier.xaml
  /// </summary>
  public partial class LevelEditorRequirementModifier : UserControl
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(LevelEditorRequirementModifier), new PropertyMetadata(default(int), ValuePropertyChangedCallback));
    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(string), typeof(LevelEditorRequirementModifier), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(int), typeof(LevelEditorRequirementModifier), new PropertyMetadata(default(int)));
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(LevelEditorRequirementModifier), new PropertyMetadata(default(bool)));
    public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register(nameof(IconColor), typeof(Brush), typeof(LevelEditorRequirementModifier), new PropertyMetadata(default(Brush)));
    public static readonly DependencyProperty ButtonApplyIconProperty = DependencyProperty.Register(nameof(ButtonApplyIcon), typeof(PackIconKind), typeof(LevelEditorRequirementModifier), new PropertyMetadata(default(PackIconKind)));

    private static void ValuePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs evt)
    {
      ((LevelEditorRequirementModifier)dependencyObject).Value = (int)evt.NewValue;
    }

    public Func<int, int> CalculateAction;

    public LevelEditorRequirementModifier()
    {
      InitializeComponent();
    }

    public int Value
    {
      get { return (int)GetValue(ValueProperty); }
      set
      {
        if (Value == value) return;
        var last = Value;
        HistoryContext.Execute(() =>
        {
          SetValue(ValueProperty, value);
        }, () =>
        {
          SetValue(ValueProperty, last);
        }, $"change modifier id:{Id}, level:'{last}'->'{value}'");
      }
    }

    public IHistoryExecuter HistoryContext { get; set; }

    public int Id
    {
      get { return (int)GetValue(IdProperty); }
      set
      {
        SetValue(IdProperty, value);
        Type = AppSettings.Setting.GetModifierType(value).ToString();
        IconColor = ModifierTypeToColor.ToColor(AppSettings.Setting.GetModifierType(value));
      }
    }

    public string Type
    {
      get { return (string)GetValue(TypeProperty); }
      set { SetValue(TypeProperty, value); }
    }

    public bool IsChecked
    {
      get { return (bool)GetValue(IsCheckedProperty); }
      set { SetValue(IsCheckedProperty, value); }
    }

    public Brush IconColor
    {
      get { return (Brush)GetValue(IconColorProperty); }
      set { SetValue(IconColorProperty, value); }
    }

    public PackIconKind ButtonApplyIcon
    {
      get { return (PackIconKind)GetValue(ButtonApplyIconProperty); }
      set { SetValue(ButtonApplyIconProperty, value); }
    }


    public MaterialDesignThemes.Wpf.PackIconKind CheckIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Check; } }
    public MaterialDesignThemes.Wpf.PackIconKind AlertIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Alert; } }


    private void IsValid()
    {
      if (Value.ToString() == Input.Text)
      {
        ButtonApplyIcon = CheckIcon;
      }
      else
      {
        ButtonApplyIcon = AlertIcon;
      }
    }

    private void Input_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }

    private void Button_Apply_OnClick(object sender, RoutedEventArgs e)
    {
      IsValid();
    }

    private void Input_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      IsValid();
    }

    private void Button_Calculate_OnClick(object sender, RoutedEventArgs e)
    {
      if (CalculateAction != null)
      {
        Value = CalculateAction(Id);
        Input.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
      }
    }
  }
}
