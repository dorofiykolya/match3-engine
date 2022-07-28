using System.Collections.Generic;
using Match3.Engine.Matches;

namespace Match3.Engine.Levels
{
  public abstract class SwapCombinationActivator
  {
    /// <summary>
    /// пользовательская активация ячеек при перемещении
    /// </summary>
    /// <param name="engineState">состояние игры</param>
    /// <param name="context">контекст для активации и генерации предметов</param>
    /// <param name="fromItem">ячейка с которой было произведено перемещение</param>
    /// <param name="toItem">ячейка в которую было произведено перемещение</param>
    /// <param name="combinations">комбинации, которые были найдены</param>
    /// <param name="result">результат для визуализации, может быть null (если расчет производится без учета визуализации)</param>
    public abstract void ActivateSwap(EngineState engineState, IActivatorContext context, Tile fromItem, Tile toItem, IList<Match> combinations, ActivationResult result);
  }
}
