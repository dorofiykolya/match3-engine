namespace Match3.Engine.Descriptions.Items
{
  [System.Serializable]
  public class ItemLevelDescription
  {
    /// <summary>
    /// к-во очков получаемых за камень
    /// </summary>
    public int Score;

    /// <summary>
    /// активатор ячеек (описание в самом классе)
    /// </summary>
    public ItemActivator Activator;

    /// <summary>
    /// описание
    /// </summary>
    public string Description;
  }
}
