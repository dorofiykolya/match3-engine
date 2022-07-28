using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;
using Match3.Engine.Utils;
using Match3.Providers;
using Match3.Settings;
using Orientation = System.Windows.Controls.Orientation;
using Point = Match3.Engine.Levels.Point;

namespace Match3.Editor.Player
{
  public class PlayerEngine
  {
    private readonly LevelPlayer _view;
    private readonly PlayerCommandProcessor _commandProcessor;
    private readonly PlayerContext _context;

    private Match3Setting _setting;
    private LevelDescription _levelDescription;
    private IEngine _engine;
    private TileGridControl _tileGrid;
    private DispatcherTimer _timer;
    private int _lastTime;
    private CoroutineManager _coroutineManager;
    private bool _showSwaps;
    private SpellItem _selectedSpellItem;
    private List<Point> _positions = new List<Point>();

    public PlayerEngine(LevelPlayer view)
    {
      _view = view;
      _commandProcessor = new PlayerCommandProcessor();
      _timer = new DispatcherTimer(DispatcherPriority.Render)
      {
        Interval = TimeSpan.FromSeconds(1.0 / 60.0)
      };
      _lastTime = Environment.TickCount;
      var coroutineTick = new CoroutineTick();
      _timer.Tick += (sender, args) =>
      {
        var now = Environment.TickCount;
        var diff = now - _lastTime;
        _lastTime = now;
        if (IsPlay)
        {
          coroutineTick.RaiseTick(diff / 1000.0);
        }
      };
      _timer.Start();
      _coroutineManager = new CoroutineManager(coroutineTick);
      _context = new PlayerContext(_coroutineManager);
    }

    public PlayerContext Context { get { return _context; } }
    public bool AllowSwap { get { return Context.QueueIsEmpty && IsPlay; } }
    public bool IsPlay { get; set; }

    public bool ShowSwaps
    {
      get { return _showSwaps; }
      set
      {
        _showSwaps = value;
        Context.Enqueue(ExecuteShowSwaps());
      }
    }

    public bool IsUseSpell { get { return _selectedSpellItem != null; } }

    public void LoadLevel(Match3Setting setting, LevelDescription level, int energy)
    {
      Match3ProviderFactory providersFactory = new Match3ProviderFactory();
      var providers = providersFactory.Create(setting);
      var factory = new Match3Factory(providers, EngineEnvironment.DefaultDebugClient | EngineEnvironment.AvailableAllSpells);
      var engine = factory.CreateInstance(level, energy, new Spell[0]);
      _engine = engine;

      var bounds = BoundsUtils.Calculate(level.Tiles);
      _view.TileGridControl.InitializeByEngine(this, bounds.Width, bounds.Height);

      _view.SetLevelBounds(bounds);

      var spells = providers.SpellDescriptionProvider.Collection;
      var spellContainer = new StackPanel();
      spellContainer.Orientation = Orientation.Horizontal;
      _view.Spells.Children.Add(spellContainer);

      foreach (var spellDescription in spells)
      {
        var levelIndex = 0;
        foreach (var levelDescription in spellDescription.Levels)
        {
          var item = new SpellItem
          {
            Id = spellDescription.Id,
            Level = levelIndex,
            Label = spellDescription.Id + ":" + (levelIndex + 1),
            ToolTip = $"value:{levelDescription.Value}\nenergy:{levelDescription.Energy}\n{AppSettings.Setting.GetDescription(spellDescription.Description)}"
          };
          item.Click += (sender, args) =>
          {
            if (_selectedSpellItem == item)
            {
              item.Selected = false;
              _selectedSpellItem = null;
              _positions.Clear();
            }
            else
            {
              if (_selectedSpellItem != null)
              {
                _selectedSpellItem.Selected = false;
              }
              _view.TileGridControl.Unselect();
              _selectedSpellItem = item;
              _positions.Clear();
              item.Selected = true;
            }
          };
          spellContainer.Children.Add(item);
          if (spellContainer.Children.Count >= 10)
          {
            spellContainer = new StackPanel();
            spellContainer.Orientation = Orientation.Horizontal;
            _view.Spells.Children.Add(spellContainer);
          }
          levelIndex++;
        }
      }

      Process();
    }

    private void Process()
    {
      Context.Enqueue(ExecuteHideSwaps());

      while (_engine.Output.Count != 0)
      {
        var evt = _engine.Output.Dequeue();
        _commandProcessor.Execute(evt, Context, _view);
        _engine.Output.ReleaseToPool(evt);
      }
      Context.Enqueue(ExecuteShowSwaps());
    }

    private IEnumerator ExecuteShowSwaps()
    {
      yield return null;
      if (ShowSwaps)
      {
        var swaps = new List<Swap>();
        if (_engine.State.AvailableSwaps(swaps))
        {
          foreach (var swap in swaps)
          {
            _view.TileGridControl.GetTile(swap.First).SwapVisible = true;
            _view.TileGridControl.GetTile(swap.Second).SwapVisible = true;
          }
        }
      }
      else
      {
        foreach (var tile in _view.TileGridControl.Tiles)
        {
          tile.SwapVisible = false;
        }
      }
    }

    private IEnumerator ExecuteHideSwaps()
    {
      yield return null;
      foreach (var tile in _view.TileGridControl.Tiles)
      {
        tile.SwapVisible = false;
      }
    }

    public void Continue(int swaps)
    {
      _engine.Continue(swaps);
      Process();
    }

    public bool Backward()
    {
      var result = _engine.Backward();
      Process();
      return result;
    }

    public void Swap(Swap swap)
    {
      _engine.AddAction(new SwapInputAction
      {
        Swap = swap,
        Tick = _engine.Tick + 1
      });
      Context.Enqueue(EngineNextTick());
    }

    public void AddEnrgy(int energy)
    {
      _engine.AddAction(new AddEnergyInputAction
      {
        Energy = energy,
        Tick = _engine.Tick + 1
      });
      Context.Enqueue(EngineNextTick());
    }

    private IEnumerator EngineNextTick()
    {
      yield return null;
      _engine.FastForward(_engine.Tick + 1);
      Process();
    }

    public bool CanSwap(Swap swap)
    {
      return _engine.State.CanSwap(swap);
    }

    public void Dispose()
    {
      _timer.Stop();
    }

    public void UseSpell(Point position)
    {
      if (_selectedSpellItem != null)
      {
        _positions.Add(position);

        var usePoints = _engine.Configuration.Providers.SpellDescriptionProvider.Get(_selectedSpellItem.Id).UsePoints;
        if (usePoints <= _positions.Count)
        {
          Context.Enqueue(EngineUseSpell(_positions.ToArray()));
          Context.Enqueue(EngineNextTick());
          _positions.Clear();
        }
      }
    }

    private IEnumerator EngineUseSpell(Point[] positions)
    {
      yield return null;
      _engine.AddAction(new UseSpellInputAction
      {
        Id = _selectedSpellItem.Id,
        Level = _selectedSpellItem.Level,
        Positions = positions,
        Tick = _engine.Tick + 1,
        Type = UseSpellType.Suite
      });
    }
  }
}
