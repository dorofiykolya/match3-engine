using System;
using System.Collections.Generic;
using System.IO;
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
using MahApps.Metro.Controls;
using Match3.Editor.Utils;
using Match3.Engine.Descriptions.Levels;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для StartWindow.xaml
  /// </summary>
  public partial class StartWindow : Window
  {
    public StartWindow()
    {
      this.Loaded += OnLoaded;
      InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      RecentlyFiles.Instance.LoadRecently();
      RecentlyFiles.Instance.PropertyChanged += (o, args) =>
      {
        Execute(UpdatePlaceholders);
      };
      Execute(UpdatePlaceholders);
    }

    private void UpdatePlaceholders()
    {
      RecentlyPanel.Children.Clear();
      if (RecentlyFiles.Instance.RecentlyOpened.Count != 0)
      {
        foreach (var jsonFile in RecentlyFiles.Instance.RecentlyOpened)
        {
          var placeholder = new RecentlyPlaceholder();
          placeholder.Loaded += (o, args) =>
          {
            placeholder.File = jsonFile;
            placeholder.FileName.Text = System.IO.Path.GetFileNameWithoutExtension(jsonFile.Path);
            placeholder.FilePath.Text = jsonFile.Path;
            placeholder.Time.Text = jsonFile.Time.ToString();
          };
          RecentlyPanel.Children.Add(placeholder);
        }
      }
      else
      {
        RecentlyPanel.Children.Add(new Card()
        {
          Padding = new Thickness(16),
          Margin = new Thickness(10),
          Content = new TextBlock()
          {
            Text = "empty",
          }
        });
      }
    }

    private void ButtonOpenEdit_OnClick(object sender, RoutedEventArgs e)
    {
      Execute(() =>
      {
        var dialog = new OpenFileDialog();
        dialog.Filter = LevelEditorUtils.FileExtensionFilter;
        dialog.FileOk += (o, args) =>
        {
          if (!args.Cancel)
          {
            try
            {
              var file = dialog.FileName;
              if (LevelEditorUtils.OpenLevel(file))
              {
                RecentlyFiles.Instance.AddRecently(file);
              }
            }
            catch (Exception exception)
            {
              MessageBox.Show(this, exception.Message);
            }
          }
        };
        dialog.ShowDialog(this);
      });
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
      Execute(() => { LevelEditorUtils.OpenNew(this); });
    }

    private void ButtonOpenPlay_OnClick(object sender, RoutedEventArgs e)
    {
      Execute(() =>
      {
        var dialog = new OpenFileDialog();
        dialog.Filter = LevelEditorUtils.FileExtensionFilter;
        dialog.FileOk += (o, args) =>
        {
          if (!args.Cancel)
          {
            try
            {
              var file = dialog.FileName;
              if (LevelEditorUtils.PlayLevel(file))
              {
                RecentlyFiles.Instance.AddRecently(file);
              }
            }
            catch (Exception exception)
            {
              MessageBox.Show(this, exception.Message);
            }
          }
        };
        dialog.ShowDialog(this);
      });
    }

    private async void Execute(Action action)
    {
      RootDialog.IsOpen = true;
      await Task.Delay(500);
      action();
      await Task.Delay(500);
      RootDialog.IsOpen = false;
    }
  }
}
