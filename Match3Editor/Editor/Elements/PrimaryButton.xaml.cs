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

namespace Match3.Editor.Elements
{
  /// <summary>
  /// Логика взаимодействия для ApplyButton.xaml
  /// </summary>
  public partial class PrimaryButton : Button, INotifyPropertyChanged
  {
    private string _label;
    private PackIconKind _iconKind;

    public PrimaryButton()
    {
      InitializeComponent();
    }

    public string Label
    {
      get { return _label; }
      set
      {
        if (_label != value)
        {
          _label = value;
          OnPropertyChanged(nameof(Label));
        }
      }
    }

    public PackIconKind IconKind
    {
      get { return _iconKind; }
      set
      {
        if (_iconKind != value)
        {
          _iconKind = value;
          OnPropertyChanged(nameof(IconKind));
        }
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
