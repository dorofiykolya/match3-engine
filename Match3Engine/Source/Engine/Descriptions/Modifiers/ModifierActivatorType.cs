using System;

namespace Match3.Engine.Descriptions.Modifiers
{
  [Serializable, Flags]
  public enum ModifierActivatorType
  {
    /// <summary>
    /// не активируется
    /// </summary>
    None = 0,

    /// <summary>
    /// активируется только в текущей клетке
    /// </summary>
    Current = 1,

    /// <summary>
    /// активируется из соседних клеток
    /// </summary>
    Near = 1 << 1,

    /// <summary>
    /// активируется в текущей клетке и соседних
    /// </summary>
    CurrentAndNear = Current | Near,
  }
}
