using System.Collections.Generic;

namespace Match3.Engine.Levels
{
  public class ShuffleResult
  {
    public List<Swap> Shuffles = new List<Swap>();

    public class Swap
    {
      public Point From;
      public Point To;
    }
  }
}
