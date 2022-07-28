using System.Collections;
using System.Windows.Controls;
using Match3.Editor.Utils;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class PreSwapCommand : PlayerCommand<PreSwapEvent>
  {
    protected override void Execute(PreSwapEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Swap(evt, context, view, context.TimeProvider));
    }

    private IEnumerator Swap(PreSwapEvent evt, PlayerContext context, LevelPlayer view, ITimeProvider timeProvider)
    {
      yield return null;
      var fromTile = view.TileGridControl.GetTile(evt.Swap.First);
      var toTile = view.TileGridControl.GetTile(evt.Swap.Second);
      var fromItem = fromTile.Item;
      var toItem = toTile.Item;

      fromTile.Item = toItem;
      toTile.Item = fromItem;

      toItem.OwnerTile = fromTile;
      fromItem.OwnerTile = toTile;

      var time = 0.2;
      var passedTime = 0.0;
      var fromStartPosition = toTile.CanvasPosition;
      var toStartPosition = fromTile.CanvasPosition;
      while (true)
      {
        yield return null;

        passedTime += timeProvider.DeltaTime;

        var x = MathHelper.Lerp(fromStartPosition.X, Canvas.GetLeft(fromTile), passedTime / time);
        var y = MathHelper.Lerp(fromStartPosition.Y, Canvas.GetTop(fromTile), passedTime / time);
        Canvas.SetLeft(toItem, x);
        Canvas.SetTop(toItem, y);
        toItem.UpdateLayout();

        x = MathHelper.Lerp(toStartPosition.X, Canvas.GetLeft(toTile), passedTime / time);
        y = MathHelper.Lerp(toStartPosition.Y, Canvas.GetTop(toTile), passedTime / time);
        Canvas.SetLeft(fromItem, x);
        Canvas.SetTop(fromItem, y);
        fromItem.UpdateLayout();
        
        if (passedTime / time >= 1.0) yield break;
      }
    }
  }
}
