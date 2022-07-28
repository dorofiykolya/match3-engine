using System.Collections.Generic;
using Match3.Engine.InputActions;

namespace Match3.Engine
{
  public interface IEngine
  {
    /// <summary>
    /// состояние на текущий момент
    /// </summary>
    IEngineState State { get; }

    /// <summary>
    /// провайдер для событий, который происходят (для визуализации)
    /// </summary>
    IEngineOutput Output { get; }

    /// <summary>
    /// текущий шаг (тик)
    /// </summary>
    int Tick { get; }

    /// <summary>
    /// перемотать вперед на определенный шаг (тик)
    /// </summary>
    /// <param name="tick">шаг, на который нужно перемотать</param>
    /// <returns></returns>
    int FastForward(int tick);

    /// <summary>
    /// доабвить действие в очередь для обработки
    /// </summary>
    /// <param name="action">действие</param>
    void AddAction(InputAction action);

    /// <summary>
    /// действия пользователя
    /// </summary>
    IEngineActions Actions { get; }

    /// <summary>
    /// конфигурация уровня и провайдеры данных
    /// </summary>
    Configuration Configuration { get; }

    /// <summary>
    /// действия, которые совершил пользователь
    /// </summary>
    IEnumerable<InputAction> PlayedActions { get; }

    /// <summary>
    /// продолжить игру
    /// </summary>
    /// <param name="additionalSwaps">дополнительное к-во шагов</param>
    void Continue(int additionalSwaps);

    /// <summary>
    /// перемотать на одно действие назад (игра инициализируется с самого начала и переходит на предыдущий шаг)
    /// </summary>
    /// <returns>если действие успешное (к примеру если перемотать на начало то отменить действия уже некуда)</returns>
    bool Backward();
  }
}
