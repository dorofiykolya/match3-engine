using System.Collections;
using Match3.Editor.Utils;
using Match3.Editor.Utils.Coroutine;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class GenerateCommand : PlayerCommand<GenerateEvent>
  {
    protected override void Execute(GenerateEvent evt, PlayerContext context, LevelPlayer view)
    {
      var sequence = context.CreateSequence();
      foreach (var item in evt.Items)
      {
        sequence.Add(GenerateItem(item, view, context.TimeProvider));
      }
      context.Enqueue(sequence);
    }

    private IEnumerator GenerateItem(GenerateEvent.GenerateItem evt, LevelPlayer view, ITimeProvider timeProvider)
    {
      yield return null;
      var tile = view.TileGridControl.GetTile(evt.ToTile);
      view.TileGridControl.AddItem(tile, evt.Item);

      var item = tile.Item;

      var startScale = .1;
      var finishScale = 1;

      item.Scale = startScale;

      var time = 0.1;
      var passedTime = 0.0;
      while (true)
      {
        yield return null;
        passedTime += timeProvider.DeltaTime;
        var ratio = passedTime / time;
        item.Scale = MathHelper.Lerp(startScale, finishScale, ratio);
        if (ratio >= 1.0)
        {
          yield break;
        }
      }
    }
  }
}
