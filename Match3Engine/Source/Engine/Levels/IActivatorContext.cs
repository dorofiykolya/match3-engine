namespace Match3.Engine.Levels
{
  public interface IActivatorContext
  {
    /// <summary>
    /// сгенерировать предмет в ячейке
    /// </summary>
    /// <param name="item">предмет</param>
    /// <param name="pivot">в какой ячейке</param>
    /// <param name="result">результат для визуализации, может быть null</param>
    void Generate(Item item, Point pivot, ActivationResult result);

    /// <summary>
    /// ативировать ячейку
    /// </summary>
    /// <param name="position">позиция ячейки</param>
    /// <param name="pivot">инициатор</param>
    /// <param name="swap">перестановка, может быть null</param>
    /// <param name="result">результат для визуализации, может быть null</param>
    void Activate(Point position, Point pivot, Swap swap, ActivationResult result);

    /// <summary>
    /// отметить ячейка
    /// </summary>
    /// <param name="position">позиция ячейки</param>
    /// <returns></returns>
    bool CheckPosition(Point position);
  }
}
