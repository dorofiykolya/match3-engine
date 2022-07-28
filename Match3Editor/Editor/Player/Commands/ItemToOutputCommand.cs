using System.Collections;
using Match3.Editor.Player;
using Match3.Editor.Utils;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Player.Commands
{
  public class ItemToOutputCommand : PlayerCommand<ItemToOuputEvent>
  {
    protected override void Execute(ItemToOuputEvent evt, PlayerContext context, LevelPlayer view)
    {
      var activateSequence = context.CreateSequence();
      activateSequence.Add(RemoveItem(view, evt.Position, context.TimeProvider));
      context.Enqueue(activateSequence);
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
