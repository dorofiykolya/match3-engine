using System;
using System.Collections.Generic;
using System.Reflection;
using Match3.Engine.InputActions;
using Match3.Engine.OutputEvents;

namespace Match3.Engine
{
  /// <summary>
  /// главный класс движка
  /// </summary>
  public class Engine : IEngine
  {
    public const int Version = 1;

    private const int MaxTickSteps = 10000;

    private readonly Configuration _configuration;
    private readonly Queue<InputAction> _playedActions;
    private readonly EngineOutput _output;
    private readonly EngineProcessor _processor;
    private readonly EngineModules _modules;
    private readonly EngineState _state;
    private readonly EngineActions _actions;

    /// <summary>
    /// констурктор движка, расширяет интерфейс IEngine
    /// </summary>
    /// <param name="configuration">конфигурация уровня и провайдеры данных</param>
    public Engine(Configuration configuration)
    {
      _configuration = configuration;
      _playedActions = new Queue<InputAction>();
      _output = new EngineOutput();
      _state = new EngineState(_configuration, _output);
      _processor = new EngineProcessor(_configuration.Providers.CommandsProvider);
      _modules = new EngineModules(_configuration.Providers.ModulesProvider);
      _actions = new EngineActions();

      if (configuration.Tick < 0) throw new ArgumentException("tick не может быть отрицательным");

      if (_configuration.Actions != null)
      {
        foreach (var action in _configuration.Actions)
        {
          AddAction(action);
        }
      }

      if (configuration.Environment.IsGenerateOutputEvents())
      {
        // первое событие о создании и инициализации
        _output.EnqueueByFactory<CreateEvent>(0).InitializeFrom(_state);
      }

      FastForward(Math.Max(1, _configuration.Tick));
    }

    /// <summary>
    /// окружение
    /// </summary>
    public EngineEnvironment Environment
    {
      get { return _configuration.Environment; }
    }

    /// <summary>
    /// конфигурация уровня и провайдеры данных
    /// </summary>
    public Configuration Configuration
    {
      get { return _configuration; }
    }

    /// <summary>
    /// модули обработки шагов (тики)
    /// </summary>
    public EngineModules Modules
    {
      get { return _modules; }
    }

    /// <summary>
    /// действия пользователя
    /// </summary>
    public IEngineActions Actions
    {
      get { return _actions; }
    }

    /// <summary>
    /// действия, которые совершил пользователь
    /// </summary>
    public IEnumerable<InputAction> PlayedActions
    {
      get { return new Queue<InputAction>(_playedActions); }
    }

    /// <summary>
    /// перемотать вперед на определенный шаг (тик)
    /// </summary>
    /// <param name="tick">шаг, на который нужно перемотать</param>
    /// <returns></returns>
    public int FastForward(int tick)
    {
      if (_state.IsFinished)
      {
        return _state.Tick;
      }

      var finish = false;
      if (tick >= _configuration.MaxTicks && _configuration.Environment.IsDebug())
      {
        tick = _configuration.MaxTicks;
        finish = true;
      }
      var currentTick = _state.Tick;
      while (++currentTick <= tick)
      {
        _state.Validate();
        _state.UpdateTick(currentTick);

        _state.UpdateTickState(EngineTickState.PreTick);
        _state.Invalidate();
        var step = 0;
        while (!_state.IsFinished && _state.IsInvalid && (step < MaxTickSteps))
        {
          _state.Validate();
          _modules.PreTick(this, currentTick, _state, step);
          ++step;
        }
        if (step == MaxTickSteps) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + string.Format(": PreTick рекурсия, максимальная вложеность:{0}", MaxTickSteps));

        _state.UpdateTickState(EngineTickState.Command);

        InputAction currentAction;
        if (!_state.IsFinished && _actions.Count != 0 && (currentAction = _actions.Peek()).Tick <= currentTick)
        {
          _playedActions.Enqueue(_actions.Dequeue());
          _processor.Execute(currentAction, this, _state);
        }

        _state.UpdateTickState(EngineTickState.PostTick);
        _state.Invalidate();
        step = 0;
        while (!_state.IsFinished && _state.IsInvalid && (step < MaxTickSteps))
        {
          _state.Validate();
          _modules.PostTick(this, currentTick, _state, step);
          ++step;
        }
        if (step == MaxTickSteps) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + string.Format(": PostTick рекурсия, максимальная вложеность:{0}", MaxTickSteps));

        _state.UpdateTickState(EngineTickState.FinalizeTick);
        _state.Invalidate();
        step = 0;
        while (!_state.IsFinished && _state.IsInvalid && (step < MaxTickSteps))
        {
          _state.Validate();
          _modules.FinalizeTick(this, currentTick, _state, step);
          ++step;
        }
        if (step == MaxTickSteps) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + string.Format(": FinalizeTick рекурсия, максимальная вложеность:{0}", MaxTickSteps));

        if (_state.IsFinished)
        {
          break;
        }

        _state.UpdateTickState(EngineTickState.None);
      }
      if (!_state.IsFinished)
      {
        _state.UpdateTick(tick);
        if (finish)
        {
          _state.Finish(EngineFinishReason.MaxTick);
        }
      }
      return _state.Tick;
    }

    /// <summary>
    /// перемотать на один шаг назад
    /// </summary>
    public bool Backward()
    {
      var tick = Tick;
      if (tick > 1)
      {
        var nextTick = tick - 1;
        StartOver();
        FastForward(nextTick);
        while (_actions.Count != 0)
        {
          _actions.Dequeue();
        }
        while (Output.Count != 0)
        {
          Output.ReleaseToPool(Output.Dequeue());
        }
        if (Environment.IsGenerateOutputEvents())
        {
          // сообщить, что уровень нужно обнулить
          _output.EnqueueByFactory<StartOverEvent>(0);
          // первое событие о создании и инициализации
          _output.EnqueueByFactory<CreateEvent>(0).InitializeFrom(_state);
        }
        return true;
      }

      return false;
    }

    /// <summary>
    /// продолжить игру
    /// </summary>
    /// <param name="additionalSwaps">дополнительное к-во шагов</param>
    public void Continue(int additionalSwaps)
    {
      if (additionalSwaps <= 0) throw new ArgumentException("additionalSwaps не может быть 0 или меньше");
      if (!_state.IsFinished)
      {
        throw new InvalidOperationException("нельзя продолжить игру, если игра не закончилась");
      }
      if (_state.FinishReason != EngineFinishReason.SwapsEnded)
      {
        throw new InvalidOperationException("нельзя продолжить игру, если причина завершения игры не является: EngineFinishReason.SwapsEnded");
      }
      _state.Continue(additionalSwaps);
    }

    /// <summary>
    /// доабвить действие в очередь для обработки
    /// </summary>
    /// <param name="action">действие</param>
    public void AddAction(InputAction action)
    {
      if (action.Tick <= Tick) throw new ArgumentException(string.Format("{2} новое действие не можеть быть меньше или равно чем уже пройденый тик игры. Тик действия:{0}. Тик игры:{1}", action.Tick, Tick, MethodBase.GetCurrentMethod().Name));
      _actions.Enqueue(action);
    }

    /// <summary>
    /// состояние на текущий момент
    /// </summary>
    public IEngineState State
    {
      get { return _state; }
    }

    /// <summary>
    /// текущий шаг (тик)
    /// </summary>
    public int Tick
    {
      get { return _state.Tick; }
    }

    /// <summary>
    /// провайдер для событий, который происходят (для визуализации)
    /// </summary>
    public IEngineOutput Output
    {
      get { return _output; }
    }

    /// <summary>
    /// добавить ивент для CLIENT
    /// </summary>
    /// <param name="evt"></param>
    public void Enqueue(OutputEvent evt)
    {
      _output.Enqueue(evt);
    }

    /// <summary>
    /// добавить ивент для CLIENT
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tick"></param>
    /// <returns></returns>
    public T EnqueueByFactory<T>(int tick) where T : OutputEvent, new()
    {
      return _output.EnqueueByFactory<T>(tick);
    }

    /// <summary>
    /// начать уровень сначала (переинициализация данных)
    /// </summary>
    public void StartOver()
    {
      while (_actions.Count != 0)
      {
        _actions.Dequeue();
      }

      _state.StartOverInternal();

      foreach (var action in _playedActions)
      {
        _actions.Enqueue(action);
      }

      _playedActions.Clear();
    }
  }
}
