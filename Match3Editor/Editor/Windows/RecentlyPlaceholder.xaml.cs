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

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для RecentlyPlaceholder.xaml
  /// </summary>
  public partial class RecentlyPlaceholder : UserControl
  {
    public RecentlyPlaceholder()
    {
      InitializeComponent();
    }

    public RecentlyFiles.RecentlyFile File { get; set; }

    private void Button_Play_OnClick(object sender, RoutedEventArgs e)
    {
      Execute(() =>
      {
        LevelEditorUtils.PlayLevel(FilePath.Text);
      });
    }

    private void Button_Edit_OnClick(object sender, RoutedEventArgs e)
    {
      Execute(() =>
      {
        LevelEditorUtils.OpenLevel(FilePath.Text);
      });
    }

    private async void Execute(Action action)
    {
      DialogHost.IsOpen = true;
      await Task.Delay(1000);
      action();
      await Task.Delay(1000);
      DialogHost.IsOpen = false;
    }

    private void Button_DeleteFromList_OnClick(object sender, RoutedEventArgs e)
    {
      RecentlyFiles.Instance.RemoveRecently(File);
    }
  }
}
