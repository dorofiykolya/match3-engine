using System.Collections;
using Match3.Editor.Utils;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class ActivateCommand : PlayerCommand<ActivateEvent>
  {
    protected override void Execute(ActivateEvent evt, PlayerContext context, LevelPlayer view)
    {
      var activateSequence = context.CreateSequence();
      var generateSequence = context.CreateSequence();
      foreach (var action in evt.Actions)
      {
        switch (action.Status)
        {
          case ActivateEvent.Status.Activated:
            activateSequence.Add(RemoveItem(view, action.Position, context.TimeProvider));
            break;
          case ActivateEvent.Status.Generated:
            generateSequence.Add(AddItem(view, action.Position, action.Item));
            break;
        }
      }
      context.Enqueue(activateSequence);
      context.Enqueue(generateSequence);
    }

    private IEnumerator AddItem(LevelPlayer view, Point position, Item item)
    {
      yield return null;
      var tile = view.TileGridControl.GetTile(position);
      view.TileGridControl.AddItem(tile, item);
    }

    private IEnumerator RemoveItem(LevelPlayer view, Point position, ITimeProvider timeProvider)
    {
      yield return null;
      var tile = view.TileGridControl.GetTile(position);
      var item = tile.Item;
      var time = 0.1;
      var passedTime = 0.0;

      while (true)
      {
        yield return null;
        passedTime += timeProvider.DeltaTime;
        var ratio = passedTime / time;
        item.Scale = MathHelper.Lerp(1, .1, ratio);
        if (ratio >= 1)
        {
          view.TileGridControl.RemoveItem(position);
          yield break;
        }
      }
    }
  }
}
