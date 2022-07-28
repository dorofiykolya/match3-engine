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
using Match3.Engine.OutputEvents;
using Match3Editor.Annotations;

namespace Match3.Editor.Player
{
  /// <summary>
  /// Логика взаимодействия для RequirementPlayerItem.xaml
  /// </summary>
  public partial class RequirementPlayerItem : UserControl, INotifyPropertyChanged
  {
    private Brush _color = Brushes.AliceBlue;

    public int Id;
    public int Level;
    public int Value;

    public RequirementPlayerItem()
    {
      InitializeComponent();
      DataContext = this;
    }

    public void SetItem(CreateEvent.RequirementInfo info)
    {
      Id = info.Id;
      Level = info.Level;
      Value = info.Value;
      RequirementName.Content = info.Id + ":" + info.Level;
      Requirement.Content = info.Value.ToString();
      Current.Content = "0";
      ColorBrush = ItemToColor.ToColor(info.Id);
    }

    public void SetValue(int value)
    {
      Current.Content = (Value - value).ToString();
    }

    public Brush ColorBrush
    {
      get { return _color; }
      set
      {
        _color = value;
        OnPropertyChanged(nameof(ColorBrush));
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
