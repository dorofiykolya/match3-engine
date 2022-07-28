namespace Match3.Engine.Levels
{
  public abstract class CombinationActivator
  {
    /// <summary>
    /// пользовательская активация ячеек
    /// </summary>
    /// <param name="engineState">состояние игры</param>
    /// <param name="context">контекст для активации и генерации предметов</param>
    /// <param name="tile">ячека, которую нужно активировать</param>
    /// <param name="initiator">инициатор активирования</param>
    /// <param name="swap">перемещение, может быть null</param>
    /// <param name="result">результат для визуализации, может быть null (если расчет производится без учета визуализации)</param>
    public abstract void Activate(EngineState engineState, IActivatorContext context, Tile tile, Tile initiator, Swap swap, ActivationResult result);
  }
}
