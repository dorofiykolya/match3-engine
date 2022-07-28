using System;

namespace Match3.Engine.Descriptions.Modifiers
{
  [Serializable]
  public class ModifierDescription
  {
    /// <summary>
    /// идентификатор
    /// </summary>
    public int Id;

    /// <summary>
    /// тип
    /// </summary>
    public ModifierType Type;

    /// <summary>
    /// описание
    /// </summary>
    public string Description;

    /// <summary>
    /// убирается при активации соседних клеток
    /// </summary>
    public ModifierActivatorType ActivationType;
  }
}
