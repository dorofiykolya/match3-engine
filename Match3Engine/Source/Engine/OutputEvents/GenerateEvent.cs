using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class GenerateEvent : OutputEvent
  {
    public List<GenerateItem> Items = new List<GenerateItem>();

    public override void Reset()
    {
      Items.Clear();
    }

    public class GenerateItem
    {
      public Direction FromEdge;
      public Position ToTile;
      public Item Item;
    }
  }
}
