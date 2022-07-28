using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class ChangeItemEvent : OutputEvent
  {
    public Queue<Data> Queue = new Queue<Data>();

    public override void Reset()
    {
      Queue.Clear();
      base.Reset();
    }

    public class Data
    {
      public Point Position;
      public Item Item;
    }
  }
}
