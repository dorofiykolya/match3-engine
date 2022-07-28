namespace Match3.Engine.Descriptions.Spells
{
  [System.Serializable]
  public class SpellLevelDescription
  {
    /// <summary>
    /// значение (универсальное)
    /// </summary>
    public int Value;

    /// <summary>
    /// к-во требуемой энергии для использования
    /// </summary>
    public int Energy;

    /// <summary>
    /// минимальное к-во (используется для задания диапазона от MinValue..Value)
    /// </summary>
    public int MinValue;
  }
}
