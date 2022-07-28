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
using System.Windows.Shapes;
using Match3.Editor.Elements;
using Match3.Editor.Utils;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для HistoryWindow.xaml
  /// </summary>
  public partial class HistoryWindow : Window
  {
    public HistoryWindow()
    {
      InitializeComponent();
      Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      var history = HistoryManager.Get(Owner);
      history.Change += () =>
      {
        Reload(history, history.UndoActions, history.RedoActions);
      };
      Reload(history, history.UndoActions, history.RedoActions);
    }

    private void Reload(HistoryManager manager, IEnumerable<HistoryManager.HistoryItem> undoActions, IEnumerable<HistoryManager.HistoryItem> redoActions)
    {
      RedoPanel.Children.Clear();
      var index = 1;
      foreach (var action in redoActions)
      {
        var button = new PrimaryButton();
        button.ToolTip = action.Description;
        button.Label = action.Description;
        button.IconKind = PackIconKind.Redo;
        var index1 = index;
        button.Click += (sender, args) =>
        {
          for (int i = 0; i < index1; i++)
          {
            manager.Redo();
          }
        };
        RedoPanel.Children.Add(button);
        index++;
      }

      UndoPanel.Children.Clear();
      index = 0;
      foreach (var action in undoActions.Reverse())
      {
        var button = new SecondaryButton();
        button.ToolTip = action.Description;
        button.Label = action.Description;
        button.IconKind = PackIconKind.Undo;
        var index1 = index;
        button.Click += (sender, args) =>
        {
          var count = UndoPanel.Children.Count - index1;
          for (int i = 0; i < count; i++)
          {
            manager.Undo();
          }
        };
        UndoPanel.Children.Add(button);
        index++;
      }
    }
  }
}
