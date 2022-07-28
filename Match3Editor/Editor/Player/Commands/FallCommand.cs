using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Match3.Editor.Utils;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class FallCommand : PlayerCommand<FallEvent>
  {
    protected override void Execute(FallEvent evt, PlayerContext context, LevelPlayer view)
    {
      var sequence = context.CreateSequence();
      foreach (var item in evt.Items)
      {
        sequence.Add(Fall(item, context, view, context.TimeProvider));
      }

      context.Enqueue(sequence);
    }

    private IEnumerator Fall(FallEvent.FallItem evt, PlayerContext context, LevelPlayer view, ITimeProvider timeProvider)
    {
      yield return null;
      var fromTile = view.TileGridControl.GetTile(evt.From);
      var toTile = view.TileGridControl.GetTile(evt.To);
      var item = fromTile.Item;
      item.OwnerTile = toTile;
      toTile.Item = item;
      fromTile.Item = null;

      var time = 0.1;
      var passedTime = 0.0;
      var startPosition = fromTile.CanvasPosition;
      var toPositionX = Canvas.GetLeft(toTile);
      var toPositionY = Canvas.GetTop(toTile);

      while (true)
      {
        yield return null;

        passedTime += timeProvider.DeltaTime;

        double ratio = passedTime / (double)time;

        var x = MathHelper.Lerp(startPosition.X, toPositionX, ratio);
        var y = MathHelper.Lerp(startPosition.Y, toPositionY, ratio);
        Canvas.SetLeft(item, x);
        Canvas.SetTop(item, y);
        item.UpdateLayout();

        if (ratio >= 1.0) yield break;
      }
    }
  }
}
