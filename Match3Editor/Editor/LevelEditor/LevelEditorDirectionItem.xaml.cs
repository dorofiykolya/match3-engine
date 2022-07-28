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
using Match3.Engine.Levels;
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorDirectionItem.xaml
  /// </summary>
  public partial class LevelEditorDirectionItem : UserControl, INotifyPropertyChanged
  {
    private Direction _direction;

    public LevelEditorDirectionItem()
    {
      InitializeComponent();
    }

    public PackIconKind UpIcon { get { return PackIconKind.ArrowUpBold; } }
    public PackIconKind DownIcon { get { return PackIconKind.ArrowDownBold; } }
    public PackIconKind LeftIcon { get { return PackIconKind.ArrowLeftBold; } }
    public PackIconKind RightIcon { get { return PackIconKind.ArrowRightBold; } }

    public Brush IconColor
    {
      get
      {
        switch (_direction)
        {
            case Direction.Bottom:
            return Brushes.White;
            case Direction.Top:
            return Brushes.Black;
            case Direction.Left:
            return Brushes.Coral;
            case Direction.Right:
            return Brushes.DeepSkyBlue;
        }
        return Brushes.Cyan;
      }
    }

    public PackIconKind Icon
    {
      get
      {
        switch (_direction)
        {
          case Direction.Bottom:
            return DownIcon;
          case Direction.Top:
            return UpIcon;
          case Direction.Left:
            return LeftIcon;
          case Direction.Right:
            return RightIcon;
        }
        return PackIconKind.ArrowAll;
      }
    }

    public Direction Direction
    {
      get { return _direction; }
      set
      {
        _direction = value;
        OnPropertyChanged(nameof(Icon));
        OnPropertyChanged(nameof(IconColor));
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
