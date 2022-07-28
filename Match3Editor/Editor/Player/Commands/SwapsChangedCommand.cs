using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class SwapsChangedCommand : PlayerCommand<SwapsChangedEvent>
  {
    protected override void Execute(SwapsChangedEvent evt, PlayerContext context, LevelPlayer view)
    {
      view.SetSwaps(evt.Used, evt.Total);
    }
  }
}
