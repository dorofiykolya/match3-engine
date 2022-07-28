namespace Match3.Engine.Descriptions.Spells
{
  [System.Serializable]
  public class SpellDescription
  {
    /// <summary>
    /// уникальный идентификатор
    /// </summary>
    public int Id;

    /// <summary>
    /// тип спела
    /// </summary>
    public SpellType Type;

    /// <summary>
    /// информация о уровнях
    /// </summary>
    public SpellLevelDescription[] Levels;

    /// <summary>
    /// описание
    /// </summary>
    public string Description;

    /// <summary>
    /// требуемый уровень игрока
    /// </summary>
    public int RequirementUserLevel;

    /// <summary>
    /// вид спела
    /// </summary>
    public SpellUseType UseType;

    /// <summary>
    /// к-во ячеек которых нужно указать для активации спела
    /// </summary>
    public int UsePoints;

    /// <summary>
    /// уровень на котором будет доступен спелл
    /// </summary>
    public int UnlockLevel;

    /// <summary>
    /// получить уровень по индексу уровня
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public SpellLevelDescription GetLevel(int level)
    {
      return Levels[level];
    }
  }
}
