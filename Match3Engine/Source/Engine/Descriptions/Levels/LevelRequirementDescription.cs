namespace Match3.Engine.Descriptions.Levels
{
  /// <summary>
  /// требования для окончания уровня
  /// </summary>
  [System.Serializable]
  public class LevelRequirementDescription
  {
    /// <summary>
    /// тип требования
    /// </summary>
    public LevelRequirementType Type;

    /// <summary>
    /// идентификатор (к примеру идентификатор камня, при типе CollectItem)
    /// используется с типом (CollectItem, GenerateItem)
    /// </summary>
    public int Id;

    /// <summary>
    /// уровень (к примеру уровень камня, при типе CollectItem)
    /// используется с типом (CollectItem, GenerateItem, Stars)
    /// при типе Stars - уровень звезды (к примеру 0-2)
    /// </summary>
    public int Level;

    /// <summary>
    /// требуемое к-во (к примеру нужно собрать 10 камней по определенному id)
    /// используется с типом (CollectItem, GenerateItem, Stars)
    /// при типе Stars - к-во очков требуемое для получение Stars этого уровня
    /// </summary>
    public int Value;
  }
}
