namespace Match3.Engine.Descriptions.Levels
{
  [System.Serializable]
  public class LevelDescription
  {
    /// <summary>
    /// цифра для генератора рандома
    /// </summary>
    public int RandomSeed;

    /// <summary>
    /// к-во доступных пользователю перемещений
    /// </summary>
    public int Swaps;

    /// <summary>
    /// требования для прохождения уровня
    /// </summary>
    public LevelRequirementDescription[] Requirements;

    /// <summary>
    /// информация ячейках на уровне
    /// </summary>
    public LevelTileDescription[] Tiles;

    /// <summary>
    /// информация о гранях на уровне
    /// </summary>
    public LevelEdgeDescription[] Edges;

    /// <summary>
    /// доступные идентификаторы предметов для генерации
    /// </summary>
    public int[] AvailableItems;
    
    /// <summary>
    /// Описание уровня
    /// </summary>
    public string Description;

    /// <summary>
    /// заголовок
    /// </summary>
    public string Title;
  }
}
