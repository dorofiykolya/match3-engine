using System.Collections;
using System.Windows;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class StreakCommand : PlayerCommand<StreakEvent>
  {
    protected override void Execute(StreakEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Do(evt));
    }

    private IEnumerator Do(StreakEvent evt)
    {
      yield return null;
      MessageBox.Show("Streak: " + evt.Matches);
    }
  }
}
