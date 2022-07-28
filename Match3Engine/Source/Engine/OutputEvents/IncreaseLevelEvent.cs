using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class IncreaseLevelEvent : OutputEvent
  {
    public readonly List<IncreaseItem> Items = new List<IncreaseItem>();

    public override void Reset()
    {
      Items.Clear();
    }

    public class IncreaseItem
    {
      public Point Position;
      public Item Item;
    }
  }
}
