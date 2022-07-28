using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class ItemToOuputEvent : OutputEvent
  {
    public Item Item;
    public Point Position;
    public Direction ToEdge;
  }
}
