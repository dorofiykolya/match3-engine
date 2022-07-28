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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Match3;
using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Utils;
using Match3.Properties;
using Match3.Providers;
using Match3.Settings;
using Match3Editor.Annotations;

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для CreateLevelWindow.xaml
  /// </summary>
  public partial class CreateLevelWindow : Window, INotifyPropertyChanged
  {
    public CreateLevelWindow()
    {
      InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
      if (WidthGrid.SelectedIndex == -1 || HeightGrid.SelectedIndex == -1) return;

      var sizes = new int[] { 4, 5, 6, 7, 8, 9, 10 };
      var editor = new LevelEditor();
      editor.Loaded += (o, args) =>
      {
        editor.GenerateLevel(sizes[WidthGrid.SelectedIndex], sizes[HeightGrid.SelectedIndex]);
      };
      editor.Show();
      Close();
    }
  }
}
