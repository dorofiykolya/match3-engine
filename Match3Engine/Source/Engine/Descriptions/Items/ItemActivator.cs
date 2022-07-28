using Match3.Engine.Levels;

namespace Match3.Engine.Descriptions.Items
{
  /// <summary>
  /// активатор ячеек
  /// пример: если матчится ячейка и она уникальным образом активирует другие ячейки (допустим горизонтально или вертикально 5 дополнительных)
  /// </summary>
  [System.Serializable]
  public class ItemActivator
  {
    /// <summary>
    /// офсет относительно активированной ячейки по которому активируются другие ячейки
    /// </summary>
    public Point[] Offsets;
  }
}
