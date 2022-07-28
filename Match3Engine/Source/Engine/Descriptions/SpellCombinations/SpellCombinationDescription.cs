using Match3.Engine.Descriptions.Spells;

namespace Match3.Engine.Descriptions.SpellCombinations
{
  [System.Serializable]
  public class SpellCombinationDescription
  {
    /// <summary>
    /// идентификатор спела
    /// </summary>
    public int SpellId;

    /// <summary>
    /// уровень спела
    /// </summary>
    public int SpellLevel;

    /// <summary>
    /// тип спела
    /// </summary>
    public SpellUseType SpellType;

    /// <summary>
    /// очередь комбинаций
    /// </summary>
    public int[] CombinationQueue;
  }
}
