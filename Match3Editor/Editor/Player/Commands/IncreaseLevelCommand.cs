using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class IncreaseLevelCommand : PlayerCommand<IncreaseLevelEvent>
  {
    protected override void Execute(IncreaseLevelEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(IncreaseLevel(evt, context, view));
    }

    private IEnumerator IncreaseLevel(IncreaseLevelEvent evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;
      foreach (var item in evt.Items)
      {
        var control = view.TileGridControl.GetTile(item.Position);
        control.Item.ItemLevel = item.Item.Level.ToString();

        yield return null;
        var totalTime = 0.25;
        var reminingTime = totalTime;
        while (true)
        {
          reminingTime -= context.TimeProvider.DeltaTime;
          if (reminingTime <= 0)
          {
            control.Item.Scale = 1;
            break;
          }
          control.Item.Scale = 1 + (1 - Math.Abs(Math.Cos(Math.PI + Math.PI * (totalTime - Math.Max(0, reminingTime))))) * 0.25;
          yield return null;
        }

      }
    }
  }
}
