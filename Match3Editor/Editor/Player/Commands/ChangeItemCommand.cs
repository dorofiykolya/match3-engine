using System;
using System.Collections;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class ChangeItemCommand : PlayerCommand<ChangeItemEvent>
  {
    protected override void Execute(ChangeItemEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Do(evt, context, view));
    }

    private IEnumerator Do(ChangeItemEvent evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;
      foreach (var data in evt.Queue)
      {
        yield return new WaitYieldCoroutine(0.5, context.TimeProvider);

        var tile = view.TileGridControl.GetTile(data.Position);
        var item = tile.Item;

        yield return null;
        var time = 1.0;
        while (true)
        {
          time -= context.TimeProvider.DeltaTime;
          if (time <= 0)
          {
            item.Scale = 1;
            item.SetContent(data.Item);
            break;
          }
          item.Scale = 1 + (1 - Math.Abs(Math.Cos(Math.PI + Math.PI * (1 - Math.Max(0, time)))));
          yield return null;
        }
      }
    }
  }
}
