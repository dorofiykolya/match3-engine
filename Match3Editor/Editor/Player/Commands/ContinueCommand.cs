using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class ContinueCommand : PlayerCommand<ContinueEvent>
  {
    protected override void Execute(ContinueEvent evt, PlayerContext context, LevelPlayer view)
    {
      view.SetSwaps(evt.Used, evt.Total);
    }
  }
}
