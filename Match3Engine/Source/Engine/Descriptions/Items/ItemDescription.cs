namespace Match3.Engine.Descriptions.Items
{
  /// <summary>
  /// Описание предмета
  /// </summary>
  [System.Serializable]
  public class ItemDescription
  {
    /// <summary>
    /// идентификатор предмета
    /// </summary>
    public int Id;

    /// <summary>
    /// тип предмета
    /// </summary>
    public ItemType Type;

    /// <summary>
    /// информация о уровнях предмета
    /// </summary>
    public ItemLevelDescription[] Levels;

    /// <summary>
    /// описание
    /// </summary>
    public string Description;

    /// <summary>
    /// получить информацию уровня по уровню предмета
    /// </summary>
    /// <param name="level">уровень предмета</param>
    /// <returns></returns>
    public ItemLevelDescription GetLevel(int level)
    {
      return Levels[level];
    }
  }
}
