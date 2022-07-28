using Match3.Engine.Levels;

namespace Match3.Engine.Descriptions.Levels
{
  [System.Serializable]
  public class LevelEdgeDescription
  {
    /// <summary>
    /// индекс грани (задается для телепортов чтобы связать их)
    /// </summary>
    public byte Index;

    /// <summary>
    /// позиция на сетке в координатах грани
    /// </summary>
    public Point Position;

    /// <summary>
    /// тип грани (генератор, стенка, телепорт...)
    /// </summary>
    public EdgeType Type;

    /// <summary>
    /// направление грани (для телепорта, генератора...)
    /// </summary>
    public Direction Direction;

    /// <summary>
    /// если грань тип-генератор, это очередь следующих предопределенных ячеек, когда они заканчиваются, генерируются случайным образом
    /// </summary>
    public Item[] Queue;
  }
}
