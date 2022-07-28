using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class ShuffleEvent : OutputEvent
  {
    public readonly List<ShuffleSwap> Swaps = new List<ShuffleSwap>();

    public override void Reset()
    {
      Swaps.Clear();
    }

    public void InitializeFrom(List<ShuffleResult.Swap> shuffles)
    {
      foreach (var shuffle in shuffles)
      {
        Swaps.Add(new ShuffleSwap
        {
          To = shuffle.To,
          From = shuffle.From
        });
      }
    }

    public class ShuffleSwap
    {
      public Point From;
      public Point To;
    }
  }
}
