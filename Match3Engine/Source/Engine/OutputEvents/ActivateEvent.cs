using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class ActivateEvent : OutputEvent
  {
    public Queue<Data> Actions = new Queue<Data>();

    public void InitializeFrom(Queue<ActivationResult.Data> queue)
    {
      foreach (var data in queue)
      {
        Actions.Enqueue(new Data
        {
          Item = data.Item.Copy(),
          Position = data.Position,
          Status = (Status)((int)data.Status),
          GenerateItem = data.GenerateItem,
          IsGenerateItem = data.IsGenerateItem,
        });
      }
    }

    public override void Reset()
    {
      Actions.Clear();
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
