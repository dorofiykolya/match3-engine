using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine
{
  /// <summary>
  /// текущее состояние игры
  /// </summary>
  public interface IEngineState
  {
    /// <summary>
    /// текущий шаг
    /// </summary>
    int Tick { get; }

    /// <summary>
    /// закончена ли игры
    /// </summary>
    bool IsFinished { get; }

    /// <summary>
    /// есть ли возможность переместить ячейки
    /// </summary>
    /// <param name="swap">ячейки</param>
    /// <returns></returns>
    bool CanSwap(Swap swap);

    /// <summary>
    /// есть ли возможность переместить ячейку по координатам в любом из направлений
    /// </summary>
    /// <param name="tile">координаты ячеки</param>
    /// <returns></returns>
    bool CanSwap(Point tile);

    /// <summary>
    /// является ли ячейка перемещаемой
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    bool IsSwapable(Point tile);

    /// <summary>
    /// получить список всех доступных перемещений
    /// </summary>
    /// <param name="result">массив, в который запишеться результат, может быть null</param>
    /// <returns>указывает найдены ли доступные перемещения</returns>
    bool AvailableSwaps(List<Swap> result = null);

    /// <summary>
    /// очки текущей игры
    /// </summary>
    IEngineScore Score { get; }

    /// <summary>
    /// максимальное к-во шагов
    /// </summary>
    int MaxSwaps { get; }

    /// <summary>
    /// к-во использованых шагов
    /// </summary>
    int Swaps { get; }

    /// <summary>
    /// к-во доступной энергии
    /// </summary>
    int Energy { get; }

    /// <summary>
    /// требования для уровня
    /// </summary>
    IEngineRequirement Requirements { get; }

    /// <summary>
    /// Причина завершения уровня
    /// </summary>
    EngineFinishReason FinishReason { get; }

    /// <summary>
    /// Доступные спелы
    /// </summary>
    EngineSpell Spells { get; }

    /// <summary>
    /// доступна ли ячейка
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    bool IsTileExist(Point position);

    /// <summary>
    /// получить предмет
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    Item GetItem(Point position);
  }
}
