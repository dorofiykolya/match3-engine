namespace Match3.Engine.Descriptions.Items
{
  [System.Serializable]
  public enum ItemType
  {
    /// <summary>
    /// не валидный
    /// </summary>
    Undefined = -1,

    /// <summary>
    /// обычный камень
    /// </summary>
    Cell = 0,

    /// <summary>
    /// бонусный камень (артефакт)
    /// </summary>
    Artifact = 1,

    /// <summary>
    /// ячейка, которой пофиг на ИД при свапе
    /// </summary>
    UniversalSwapCell = 2,
  }
}
