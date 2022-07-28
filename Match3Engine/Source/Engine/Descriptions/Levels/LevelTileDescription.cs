using Match3.Engine.Levels;

namespace Match3.Engine.Descriptions.Levels
{
  [System.Serializable]
  public class LevelTileDescription
  {
    /// <summary>
    /// позиция ячейки на сетке
    /// </summary>
    public Point Position;

    /// <summary>
    /// направление гравитации
    /// </summary>
    public Direction Direction;

    /// <summary>
    /// камень, который находится в данной ячейке
    /// </summary>
    public Item Item;

    /// <summary>
    /// тип ячейки
    /// </summary>
    public TileType Type;

    /// <summary>
    /// модификаторы ячейки
    /// </summary>
    public Modifier[] Modifiers;
  }
}
