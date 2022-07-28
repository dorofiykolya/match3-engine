using System.Collections;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class PostSwapCommand : PlayerCommand<PostSwapEvent>
  {
    protected override void Execute(PostSwapEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Swap(evt, view));
    }

    private IEnumerator Swap(PostSwapEvent evt, LevelPlayer view)
    {
      yield return null;
      view.SetSwaps(evt.Used, evt.Total);
    }
  }
}
