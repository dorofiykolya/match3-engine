using System.Collections.Generic;

namespace Match3.Engine.Levels
{
  public class ActivationResult
  {
    public Queue<Data> Queue = new Queue<Data>();

    public void AddActivated(Point position, Item item, Item generateItem, Point generateItemPosition)
    {
      Queue.Enqueue(new Data
      {
        Item = item,
        Position = position,
        GenerateItem = generateItemPosition,
        IsGenerateItem = generateItem != null,
        Status = Status.Activated
      });
    }

    public void AddGenerated(Point position, Item item)
    {
      Queue.Enqueue(new Data
      {
        Item = item,
        Position = position,
        Status = Status.Generated
      });
    }

    public enum Status
    {
      Activated,
      Generated
    }

    public class Data
    {
      public Status Status;
      public Point Position;
      public Item Item;
      public Point GenerateItem;
      public bool IsGenerateItem;
    }
  }
}
