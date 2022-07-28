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
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Match3.Editor.Player;
using Match3.Editor.Utils;
using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Shareds.Providers;
using Match3.Providers;
using Match3.Settings;
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для LevelPlayer.xaml
  /// </summary>
  public partial class LevelPlayer : Window, INotifyPropertyChanged
  {
    private PlayerEngine _playerEngine;
    public readonly Dictionary<LevelRequirementType, List<RequirementPlayerItem>> RequirementsPlaceholders = new Dictionary<LevelRequirementType, List<RequirementPlayerItem>>();
    private bool _isPlay;
    private bool _showSwaps;

    public LevelPlayer()
    {
      InitializeComponent();
      Loaded += (sender, args) =>
      {
        _playerEngine = new PlayerEngine(this);
      };
      Closed += (sender, args) =>
      {
        _playerEngine.Dispose();
      };
    }

    public bool IsPlay
    {
      get { return _isPlay; }
      set
      {
        _isPlay = value;
        _playerEngine.IsPlay = _isPlay;
        OnPropertyChanged(nameof(IsPlay));
      }
    }

    public bool ShowSwaps
    {
      get { return _showSwaps; }
      set
      {
        _showSwaps = value;
        _playerEngine.ShowSwaps = value;
        OnPropertyChanged(nameof(ShowSwaps));
      }
    }

    public void LoadLevel(Match3Setting setting, LevelDescription level, int energy)
    {
      _playerEngine.LoadLevel(setting, level, energy);
    }

    public void SetLevelBounds(Bounds bounds)
    {
      //Width = ((bounds.Width + 1) * CoordinateConverter.TileSize) + 108 + 50;
      //Height = ((bounds.Height + 1) * CoordinateConverter.TileSize) + 131 + 50;

      TileGridControl.Width = ((bounds.Width) * CoordinateConverter.TileSize);
      TileGridControl.Height = ((bounds.Height) * CoordinateConverter.TileSize);
      UpdateLayout();
    }

    private void TimeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (_playerEngine != null)
      {
        _playerEngine.Context.TimeProvider.TimeScale = e.NewValue;
      }
    }

    public void Finish(EngineFinishReason reason)
    {
      if (reason == EngineFinishReason.SwapsEnded)
      {
        if (MessageBox.Show(this, "Finish: " + reason + ", do you want add 5 additional swaps?", "Finish", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          _playerEngine.Continue(5);
        }
      }
      else
      {
        MessageBox.Show(this, "Finish: " + reason);
      }
    }

    public void SetRequirements(CreateEvent.RequirementInfo[] requirements)
    {
      Dictionary<LevelRequirementType, List<CreateEvent.RequirementInfo>> map = new Dictionary<LevelRequirementType, List<CreateEvent.RequirementInfo>>();
      foreach (var requirement in requirements)
      {
        List<CreateEvent.RequirementInfo> list;
        if (!map.TryGetValue(requirement.Type, out list))
        {
          map[requirement.Type] = list = new List<CreateEvent.RequirementInfo>();
        }
        list.Add(requirement);
      }

      foreach (var pair in map)
      {
        Requirements.Children.Add(new Label()
        {
          Background = Brushes.Chocolate,
          Content = pair.Key.ToString()
        });

        foreach (var info in pair.Value)
        {
          var item = new RequirementPlayerItem();
          item.SetItem(info);
          Requirements.Children.Add(item);

          List<RequirementPlayerItem> uiList;
          if (!RequirementsPlaceholders.TryGetValue(pair.Key, out uiList))
          {
            RequirementsPlaceholders[pair.Key] = uiList = new List<RequirementPlayerItem>();
          }
          uiList.Add(item);
        }
      }
    }

    public void SetSwaps(int usedSwaps, int totalSwaps)
    {
      Swaps.Content = $"Swaps: {usedSwaps}/{totalSwaps}";
    }

    public void SetEnergy(int energy)
    {
      Energy.Content = "Energy: " + energy;
    }

    public void SetScore(int score)
    {
      Score.Content = "Score: " + score;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Backward_OnClick(object sender, RoutedEventArgs e)
    {
      if (!_playerEngine.AllowSwap || !_playerEngine.Backward())
      {
        MessageBox.Show("can not backward");
      }
    }

    public void Reset()
    {
      Requirements.Children.Clear();
      RequirementsPlaceholders.Clear();
    }

    private void AddEnergy_OnClick(object sender, RoutedEventArgs e)
    {
      if (_playerEngine.AllowSwap)
      {
        _playerEngine.AddEnrgy(1);
      }
    }
  }
}
