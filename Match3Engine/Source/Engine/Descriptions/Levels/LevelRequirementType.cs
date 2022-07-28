namespace Match3.Engine.Descriptions.Levels
{
  [System.Serializable]
  public enum LevelRequirementType
  {
    /// <summary>
    /// нет типа
    /// </summary>
    None,

    /// <summary>
    /// требуется собраться предмет
    /// </summary>
    CollectItem,

    /// <summary>
    /// требуется сгенерировать предмет
    /// </summary>
    GenerateItem,

    /// <summary>
    /// требуется набрать к-во очков чтобы получить звезду
    /// </summary>
    Stars,

    /// <summary>
    /// требуется уничтожить модификаторы
    /// </summary>
    Modifier,

    /// <summary>
    /// использовать спел
    /// </summary>
    UseSpell
  }
}
