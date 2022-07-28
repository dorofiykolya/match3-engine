using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class FallEvent : OutputEvent
  {
    public List<FallItem> Items = new List<FallItem>();

    public override void Reset()
    {
      Items.Clear();
    }

    public class FallItem
    {
      public Point From;
      public Direction MoveTo;

      public Point To;
      public Direction MoveFrom;

      public MoveType MoveType;
      public Item Item;
    }
  }
}
