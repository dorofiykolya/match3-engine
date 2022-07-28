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
using Match3.Engine.Levels;
using Match3Editor.Annotations;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorEdgeWindow.xaml
  /// </summary>
  public partial class LevelEditorEdgeWindow : Window, INotifyPropertyChanged
  {
    private byte _index;
    private Direction _direction;
    private EdgeType _edgeType;
    private bool _isSecond;
    private bool _availableDirection;

    public event Action<Direction, byte> Apply;

    public LevelEditorEdgeWindow()
    {
      InitializeComponent();
    }

    public string FirstLabel { get { return _direction.IsHorizontal() ? Direction.Left.ToString() : Direction.Top.ToString(); } }
    public string SecondLabel { get { return _direction.IsHorizontal() ? Direction.Right.ToString() : Direction.Bottom.ToString(); } }

    public EdgeType EdgeType
    {
      get { return _edgeType; }
      set
      {
        if (_edgeType != value)
        {
          _edgeType = value;

          OnPropertyChanged(nameof(EdgeType));
          OnPropertyChanged(nameof(IsTeleport));
        }
      }
    }

    public Direction EdgeDirection
    {
      get { return _direction; }
      set
      {
        if (_direction != value)
        {
          _direction = value;

          if (_direction.IsHorizontal()) IsSecond = _direction == Direction.Right;
          else IsSecond = _direction == Direction.Bottom;

          OnPropertyChanged(nameof(FirstLabel));
          OnPropertyChanged(nameof(SecondLabel));
          OnPropertyChanged(nameof(EdgeDirection));
        }
      }
    }

    public byte EdgeIndex
    {
      get { return _index; }
      set
      {
        if (_index != value)
        {
          _index = value;
          OnPropertyChanged(nameof(EdgeIndex));
        }
      }
    }

    public bool IsSecond
    {
      get { return _isSecond; }
      set
      {
        if (_isSecond != value)
        {
          _isSecond = value;
          OnPropertyChanged(nameof(IsSecond));
        }
      }
    }

    public bool IsTeleport
    {
      get { return _edgeType == EdgeType.TeleportOutput || _edgeType == EdgeType.TeleportInput; }
    }

    public bool AvailableDirection
    {
      get
      {
        return _availableDirection;
      }
      set
      {
        if (_availableDirection != value)
        {
          _availableDirection = value;
          OnPropertyChanged(nameof(AvailableDirection));
        }
      }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      if (Apply != null)
      {
        Apply(_direction.IsHorizontal() ? (!IsSecond ? Direction.Left : Direction.Right) : (!IsSecond ? Direction.Top : Direction.Bottom), _index);
      }
      Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
