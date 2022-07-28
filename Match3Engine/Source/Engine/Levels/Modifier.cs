using System;

namespace Match3.Engine.Levels
{
  [Serializable]
  public class Modifier
  {
    /// <summary>
    /// идентификатор ячейки
    /// </summary>
    public int Id;

    /// <summary>
    /// уровень модификатора, к примеру, если модификатор Substrate уровень с каждый матчем на этой ячейке уменшается
    /// </summary>
    public int Level;

    /// <summary>
    /// копия обьекта
    /// </summary>
    /// <returns></returns>
    public Modifier Clone()
    {
      return new Modifier
      {
        Id = Id,
        Level = Level
      };
    }
  }
}
