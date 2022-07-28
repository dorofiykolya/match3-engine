using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Levels;
using Match3.Engine.Utils;
using System.Reflection;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.SpellCombinations;
using Match3.Engine.Matches;
using Match3.Engine.OutputEvents;
using Match3.Engine.Descriptions.Spells;

namespace Match3.Engine
{
  /// <summary>
  /// состояние на текущий момент
  /// </summary>
  public class EngineState : IEngineState, IEngineStateInvalidator, IEngineNextRandom
  {
    private readonly Configuration _configuration;
    private readonly EngineOutput _output;
    private readonly EngineStatePool _engineStatePool;

    private EngineSpell _spells;
    private EngineStreak _streak;
    private TileGrid _tileGrid;
    private TileGridActivator _tileGridActivator;
    private EngineRandom _random;
    private bool _leftOrRightToggle;
    private EngineTickState _tickState;
    private EngineScore _score;
    private EngineRequirement _requirement;
    private EngineSpellCombination _spellCombination;
    private int _swaps;
    private int _energy;
    private int _maxSwaps;
    private int _additionalSwaps;
    private int _additionalEnergy;

    public EngineState(Configuration configuration, EngineOutput output)
    {
      _configuration = configuration;
      _output = output;
      _engineStatePool = new EngineStatePool();

      InitializeState();
    }

    /// <summary>
    /// окружение
    /// </summary>
    public EngineEnvironment Environment { get { return _configuration.Environment; } }

    /// <summary>
    /// комбинация спелов
    /// </summary>
    public EngineSpellCombination SpellCombination { get { return _spellCombination; } }

    /// <summary>
    /// пулл обьктов
    /// </summary>
    public EngineStatePool Pool { get { return _engineStatePool; } }

    /// <summary>
    /// конфигурация уровня
    /// </summary>
    public Configuration Configuration { get { return _configuration; } }

    /// <summary>
    /// контейнер для ячеек
    /// </summary>
    public TileGrid TileGrid { get { return _tileGrid; } }

    /// <summary>
    /// контейнер для помеченых ячеек
    /// </summary>
    public TileGridActivator TileGridActivator { get { return _tileGridActivator; } }

    /// <summary>
    /// текущий шаг
    /// </summary>
    public int Tick { get; private set; }

    /// <summary>
    /// закончена ли игры
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// валидное текущее состояиние или нет
    /// </summary>
    public bool IsInvalid { get; private set; }

    /// <summary>
    /// нужно ли генерировать новые предметы (в конце уровня, если все требования выполнены, но остались хоты, нужно остановить генерирование)
    /// </summary>
    public bool IsGeneratorStoped { get; private set; }

    /// <summary>
    /// текущее состояние шага (тик)
    /// </summary>
    /// <value>The state of the tick.</value>
    public EngineTickState TickState { get { return _tickState; } }

    /// <summary>
    /// очки уровня
    /// </summary>
    public IEngineScore Score { get { return _score; } }

    /// <summary>
    /// провайдер ячеек
    /// </summary>
    public ITileGridProvider GridProvider { get { return TileGrid; } }

    /// <summary>
    /// требования для уровня
    /// </summary>
    public EngineRequirement Requirement { get { return _requirement; } }

    /// <summary>
    /// максимальное к-во шагов включая дополнительные ходы
    /// </summary>
    public int MaxSwaps { get { return _maxSwaps + _additionalSwaps; } }

    /// <summary>
    /// к-во использованых шагов
    /// </summary>
    public int Swaps { get { return _swaps; } }

    /// <summary>
    /// доступные спелы
    /// </summary>
    public EngineSpell Spells
    {
      get { return _spells; }
    }

    /// <summary>
    /// продолжить игру с дополнительным к-вом шагов
    /// </summary>
    /// <param name="additionalSwaps">дополнительное к-во шагов</param>
    public void Continue(int additionalSwaps)
    {
      if (additionalSwaps <= 0) throw new ArgumentException("additionalSwaps не может быть 0 или меньше");

      if (!IsFinished)
      {
        throw new InvalidOperationException("нельзя продолжить игру, если игра не закончилась");
      }
      if (FinishReason != EngineFinishReason.SwapsEnded)
      {
        throw new InvalidOperationException("нельзя продолжить игру, если причина завершения игры не является: EngineFinishReason.SwapsEnded");
      }

      _additionalSwaps += additionalSwaps;

      FinishReason = EngineFinishReason.None;
      IsFinished = false;

      if (Configuration.Environment.IsGenerateOutputEvents())
      {
        var evt = _output.EnqueueByFactory<ContinueEvent>(Tick);
        evt.Total = MaxSwaps;
        evt.Used = Swaps;
        evt.AdditionalSwaps = additionalSwaps;
      }
    }

    /// <summary>
    /// добавить спел
    /// </summary>
    /// <param name="id"></param>
    /// <param name="level"></param>
    /// <param name="count"></param>
    public void AddSpell(int id, int level, int count)
    {
      if (_tickState != EngineTickState.Command) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": только внутри состояния: \"Command\"");
      _spells.AddSpell(id, level, count);
    }

    /// <summary>
    /// к-во всего добавленных дополнительных ходов
    /// </summary>
    public int AdditionalSwaps
    {
      get { return _additionalSwaps; }
    }

    /// <summary>
    /// к-во доступной энергии
    /// </summary>
    public int Energy { get { return _energy + _additionalEnergy; } }

    /// <summary>
    /// Причина завершения уровня
    /// </summary>
    public EngineFinishReason FinishReason { get; private set; }

    /// <summary>
    /// требования для уровня
    /// </summary>
    public IEngineRequirement Requirements { get { return _requirement; } }

    /// <summary>
    /// вывод ивентов
    /// </summary>
    public EngineOutput Output { get { return _output; } }

    /// <summary>
    /// поощрения
    /// </summary>
    public EngineStreak Streak { get { return _streak; } }

    /// <summary>
    /// нужно ли автоматически активировать все предметы (исключая уровень 0)
    /// </summary>
    public bool IsAutoActivate { get; private set; }

    /// <summary>
    /// автоматически активировать все предметы (исключая уровень 0)
    /// </summary>
    public void AutoActivate()
    {
      IsAutoActivate = true;
    }

    /// <summary>
    /// остановить генерирование предметов
    /// </summary>
    public void StopGenerator()
    {
      IsGeneratorStoped = true;
    }

    /// <summary>
    /// получить направление лево-право
    /// </summary>
    /// <returns></returns>
    public Direction NextLeftOrRight()
    {
      _leftOrRightToggle = !_leftOrRightToggle;
      return _leftOrRightToggle ? Direction.Left : Direction.Right;
    }

    /// <summary>
    /// рандомное значение
    /// </summary>
    /// <returns></returns>
    public bool GetNextRandomBool()
    {
      return _random.Next(2) == 1;
    }

    /// <summary>
    /// рандомное значение
    /// </summary>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public int GetNextRandom(int maxValue)
    {
      return _random.Next(maxValue);
    }

    /// <summary>
    /// рандомное значение
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public int GetNextRandom(int minValue, int maxValue)
    {
      return _random.Next(minValue, maxValue);
    }

    /// <summary>
    /// обновить текущий шаг (тик)
    /// </summary>
    /// <param name="tick"></param>
    public void UpdateTick(int tick)
    {
      if (Tick > tick) throw new ArgumentException(string.Format("невозможно обновить тик на меньше текущего, новый тик:{0}, текущий тик:{1}", tick, Tick));
      Tick = tick;
    }

    /// <summary>
    /// обновить текущее состояние шага (тик)
    /// </summary>
    /// <param name="state">State.</param>
    public void UpdateTickState(EngineTickState state)
    {
      _tickState = state;
    }

    /// <summary>
    /// закончить игру
    /// </summary>
    public void Finish(EngineFinishReason reason)
    {
      FinishReason = reason;
      IsFinished = true;
      if (Configuration.Environment.IsGenerateOutputEvents())
      {
        _output.EnqueueByFactory<FinishEvent>(Tick).Reason = reason;
      }
    }

    /// <summary>
    /// сделать текущее состояние не валидным
    /// </summary>
    public void Invalidate()
    {
      IsInvalid = true;
    }

    /// <summary>
    /// указать текущее состояние валидным
    /// </summary>
    public void Validate()
    {
      IsInvalid = false;
    }

    /// <summary>
    /// получить предмет
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Item GetItem(Point position)
    {
      var item = TileGrid.GetTile(position);
      if(item != null)
      {
        return item.Item;
      }
      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsTileExist(Point position)
    {
      return _tileGrid.GetTile(position) != null;
    }

    /// <summary>
    /// есть ли возможность переместить ячейки
    /// </summary>
    /// <param name="swap">ячейки</param>
    /// <returns></returns>
    public bool CanSwap(Swap swap)
    {
      return _tileGrid.CanSwap(swap);
    }

    /// <summary>
    /// есть ли возможность переместить ячейку по координатам в любом из направлений
    /// </summary>
    /// <param name="tile">координаты ячеки</param>
    /// <returns></returns>
    public bool CanSwap(Point tile)
    {
      return _tileGrid.CanSwap(tile);
    }

    /// <summary>
    /// является ли ячейка перемещаемой
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsSwapable(Point tile)
    {
      var currentTile = _tileGrid.GetTile(tile);
      return currentTile != null && currentTile.IsSwapable;
    }

    /// <summary>
    /// получить список всех доступных перемещений
    /// </summary>
    /// <param name="result">массив, в который запишеться в вернется результат, если null - вернется новый массив</param>
    /// <returns>массив с результатом</returns>
    public bool AvailableSwaps(List<Swap> result = null)
    {
      return _tileGrid.AvailableSwaps(result);
    }

    /// <summary>
    /// использовать все ходы (в конце игры)
    /// </summary>
    public void FillSwaps()
    {
      if (!(_tickState == EngineTickState.PreTick || _tickState == EngineTickState.PostTick || _tickState == EngineTickState.FinalizeTick)) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": только внутри состояния: \"PreTick | PostTick | FinalizeTick\"");
      _swaps = MaxSwaps;
    }

    /// <summary>
    /// переместить ячейки (только внутри состояния Command)
    /// </summary>
    /// <returns>The swap.</returns>
    /// <param name="swap">Swap.</param>
    /// <param name="result"></param>
    public void Swap(Swap swap, ActivationResult result = null)
    {
      if (_tickState != EngineTickState.Command) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": только внутри состояния: \"Command\"");
      MatchCombinationsResult combinations;
      TileGridActivator.Activate(swap, out combinations, result);
      SpellCombination.Enqueue(swap, combinations, TileGrid);
      ++_swaps;

      if (Configuration.Environment.IsGenerateOutputEvents())
      {
        var evt = Output.EnqueueByFactory<SpellCombinationEvent>(Tick);
        evt.Available = SpellCombination.AvailableCombinationSpells().ToArray();
        evt.Possible = SpellCombination.PossibleCombinations().ToArray();
        evt.ItemsQueue = SpellCombination.ItemQueueCombinations().ToArray();
      }
    }

    /// <summary>
    /// добавить дополнительные шаги
    /// </summary>
    /// <param name="swaps">к-во добавляемых шагов</param>
    public void AddAdditionalSwaps(int swaps)
    {
      if (_tickState == EngineTickState.Command)
      {
        _additionalSwaps += swaps;
      }
      else
      {
        throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": только внутри состояния: " + EngineTickState.Command);
      }
    }

    /// <summary>
    /// добавить дополнительную энергию
    /// </summary>
    /// <param name="energy">к-во энергии</param>
    public void AddAdditionalEnergy(int energy)
    {
      if (_tickState == EngineTickState.Command)
      {
        _additionalEnergy += energy;
      }
      else
      {
        throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": только внутри состояния: " + EngineTickState.Command);
      }
    }

    /// <summary>
    /// использовать спелл (только внутри состояния Command)
    /// а так же отнимаем энергию у пользователя на текущую игровую сессию.
    /// </summary>
    public void UseSpell(UseSpell useSpell)
    {
      if (_tickState != EngineTickState.Command) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": только внутри состояния: \"Command\"");

      var provider = _configuration.Providers;

      var spell = provider.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);

      var needSkipEnergy = false;
      if (Environment.IsSkipRequiredSpellEnergy() && useSpell.Type == UseSpellType.Combination)
      {
        needSkipEnergy = Requirement.Remaining(useSpell.Id, useSpell.Level, LevelRequirementType.UseSpell) != 0;
      }

      if (Environment.IsForceSkipUseSpellEnergy())
      {
        needSkipEnergy = true;
      }

      if (useSpell.SpellType == SpellUseType.Rune)
        needSkipEnergy = true;

      if (!needSkipEnergy)
      {
        // TODO fix exception when use spell
        if (Energy < spellLevel.Energy)
          throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name +
                                              string.Format(
                                                ": недостаточно энергии для использования спела (id:{0}, level:{1}), доступно:{2}, требуется:{3}",
                                                useSpell.Id, useSpell.Level, Energy, spellLevel.Energy));

        //отнимаем энергию у пользователя на текущую игровую сессию
        RemoveEnergy(spellLevel.Energy);
      }

      if (useSpell.Type == UseSpellType.Combination)
      {
        var availableSpells = Pool.PopList<SpellCombinationDescription>();
        SpellCombination.AvailableCombinationSpells(availableSpells);
        if (!availableSpells.Any(c => c.SpellId == useSpell.Id && c.SpellLevel == useSpell.Level))
        {
          throw new InvalidOperationException(string.Format("Применяется спелл из комбинации, который в комбинациях на текущий момент не доступен id:{0}, level:{1}", useSpell.Id, useSpell.Level));
        }
        Pool.PushList(availableSpells);
      }
      else if (useSpell.Type == UseSpellType.Suite)
      {
        if (!_spells.Contains(useSpell.Id, useSpell.Level) && !_configuration.Environment.IsAvailableAllSpells())
        {
          throw new InvalidOperationException(string.Format("Нет в наличии спела из набора доступных id:{0}, level:{1}", useSpell.Id, useSpell.Level));
        }
        _spells.UseSpell(useSpell.Id, useSpell.Level);
      }

      var spellAction = provider.SpellTypeActionsProvider.Get(spell.Type);

      var isGenerateOutputEvents = Configuration.Environment.IsGenerateOutputEvents();
      if (isGenerateOutputEvents)
      {
        if (!needSkipEnergy)
        {
          Output.EnqueueByFactory<EnergyChangeEvent>(Tick).Energy = Energy;
        }

        Output.EnqueueByFactory<BeginUseSpellEvent>(Tick).UseSpell = useSpell;
      }


      spellAction.Execute(this, useSpell);

      Requirement.Dispatch(useSpell.Id, useSpell.Level, LevelRequirementType.UseSpell);
      if (isGenerateOutputEvents)
      {
        Output.EnqueueByFactory<EndUseSpellEvent>(Tick).UseSpell = useSpell;
      }

      // очистить комбинации, после использования спела
      SpellCombination.Prune();

      if (isGenerateOutputEvents && useSpell.Type == UseSpellType.Combination)
      {
        var evt = Output.EnqueueByFactory<SpellCombinationEvent>(Tick);
        evt.Available = SpellCombination.AvailableCombinationSpells().ToArray();
        evt.Possible = SpellCombination.PossibleCombinations().ToArray();
        evt.ItemsQueue = SpellCombination.ItemQueueCombinations().ToArray();
      }
    }

    /// <summary>
    /// возможные комбинации
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public List<SpellCombinationDescription> PossibleCombinations(List<SpellCombinationDescription> result)
    {
      if (result == null) result = new List<SpellCombinationDescription>();
      SpellCombination.PossibleCombinations(result);
      return result;
    }

    /// <summary>
    /// доступные спелы
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public List<SpellCombinationDescription> AvailableCombinationSpells(List<SpellCombinationDescription> result)
    {
      if (result == null) result = new List<SpellCombinationDescription>();
      SpellCombination.AvailableCombinationSpells(result);
      return result;
    }

    /// <summary>
    /// внутренний метод движка, нельзя вызывать напрямую (начать уровень сначала (переинициализация данных))
    /// </summary>
    public void StartOverInternal()
    {
      InitializeState();
    }

    /// <summary>
    /// инициализация первоначального состояния
    /// </summary>
    private void InitializeState()
    {
      _leftOrRightToggle = true;
      var level = _configuration.LevelDescription;

      _tileGrid = new TileGrid(this, level, _configuration.Providers, _configuration.Environment);
      _tileGridActivator = new TileGridActivator(this, _tileGrid, _configuration.Providers, _configuration.Environment);
      _score = new EngineScore(this, _tileGrid, _configuration.Providers, _configuration.Environment);
      _requirement = new EngineRequirement(this, level, _configuration.Providers, _configuration.Environment);
      _random = new EngineRandom(level.RandomSeed);
      _spellCombination = new EngineSpellCombination(this, _configuration.Providers.SpellCombinationsProvider.Collection, _configuration.Providers.SpellDescriptionProvider, _configuration.Environment);
      _swaps = 0;
      _maxSwaps = _configuration.LevelDescription.Swaps;
      _energy = _configuration.Energy;
      _spells = new EngineSpell(this, _configuration.Spells, _configuration.Providers);
      _streak = new EngineStreak(this);
      _additionalSwaps = 0;
      _additionalEnergy = 0;

      IsGeneratorStoped = false;
      IsFinished = false;
      IsInvalid = false;
      IsAutoActivate = false;

      Tick = 0;
    }

    private void RemoveEnergy(int energy)
    {
      _energy -= energy;
    }
  }
}
