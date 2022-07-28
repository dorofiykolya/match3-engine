using System.Collections;
using System.Windows;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class RequirementsCompleteCommand : PlayerCommand<RequirementsCompleteEvent>
  {
    protected override void Execute(RequirementsCompleteEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Do(evt));
    }

    private IEnumerator Do(RequirementsCompleteEvent evt)
    {
      yield return null;
      MessageBox.Show("Requirements Complete! AvailableSwaps:" + evt.AvailableSwaps);
    }
  }
}
