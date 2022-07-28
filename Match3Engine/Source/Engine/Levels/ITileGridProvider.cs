namespace Match3.Engine.Levels
{
  public interface ITileGridProvider
  {
    /// <summary>
    /// получить ячейку по координате ячейки (в пространстве координат ячеек)
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    Tile GetTile(Point tilePosition);

    /// <summary>
    /// получить ячейку, при перемещении
    /// </summary>
    /// <returns>результат</returns>
    /// <param name="swap"></param>
    /// <param name="tilePosition">позиция ячейки</param>
    Tile GetTileBySwap(Swap swap, Point tilePosition);

    /// <summary>
    /// получить границы ячейки по координатам границы (в пространсве координат границ)
    /// </summary>
    /// <param name="edgePosition"></param>
    /// <returns></returns>
    Edge GetEdge(Point edgePosition);

    /// <summary>
    /// получить следующую ячейку по направлению
    /// </summary>
    /// <param name="tilePosition">позиция ячейки (в пространстве координат ячеек)</param>
    /// <param name="direction">направление</param>
    /// <returns></returns>
    Tile GetTile(Point tilePosition, Direction direction);

    /// <summary>
    /// получить грань относительно ячейки
    /// </summary>
    /// <param name="tilePosition">позиция ячейки (в пространстве координат ячеек)</param>
    /// <param name="direction">направление</param>
    /// <returns></returns>
    Edge GetEdgeByTile(Point tilePosition, Direction direction);

    /// <summary>
    /// получить ячейку относительно грани
    /// </summary>
    /// <param name="edgePosition">позиция грани (в пространсве координат границ)</param>
    /// <param name="direction">направление</param>
    /// <returns></returns>
    Tile GetTileByEdge(Point edgePosition, Direction direction);

    /// <summary>
    /// получить грань по ее индексу и типу (индекс задается для телепортов, одинаковый индекс для входного и выходного телепорта)
    /// </summary>
    /// <param name="index">индекс</param>
    /// <param name="type">тип</param>
    /// <returns></returns>
    Edge GetEdgeBy(byte index, EdgeType type);
  }
}
