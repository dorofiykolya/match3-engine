using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Match3.Editor.Utils;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class ShuffleCommand : PlayerCommand<ShuffleEvent>
  {
    protected override void Execute(ShuffleEvent evt, PlayerContext context, LevelPlayer view)
    {
      var sequence = context.CreateSequence();
      foreach (var swap in evt.Swaps)
      {
        sequence.Add(Shuffle(swap, context, view));
      }
      context.Enqueue(sequence);
    }

    private IEnumerator Shuffle(ShuffleEvent.ShuffleSwap evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;
      var fromTile = view.TileGridControl.GetTile(evt.From);
      var toTile = view.TileGridControl.GetTile(evt.To);

      var fromItem = fromTile.Item;
      var toItem = toTile.Item;

      fromTile.Item = toItem;
      toTile.Item = fromItem;

      fromItem.OwnerTile = toTile;
      toItem.OwnerTile = fromTile;

      var time = 0.5;
      var passedTime = 0.0;

      var fromPosition = fromTile.CanvasPosition;
      var toPosition = toTile.CanvasPosition;

      while (true)
      {
        yield return null;

        passedTime += context.TimeProvider.DeltaTime;

        double ratio = passedTime / (double)time;
        double x, y;
        
        x = MathHelper.Lerp(fromPosition.X, toPosition.X, ratio);
        y = MathHelper.Lerp(fromPosition.Y, toPosition.Y, ratio);

        fromItem.CanvasPosition = new System.Windows.Point(x, y);

        x = MathHelper.Lerp(toPosition.X, fromPosition.X, ratio);
        y = MathHelper.Lerp(toPosition.Y, fromPosition.Y, ratio);

        toItem.CanvasPosition = new System.Windows.Point(x, y);

        toItem.UpdateLayout();
        fromItem.UpdateLayout();

        if (ratio >= 1.0) yield break;
      }
    }
  }
}
