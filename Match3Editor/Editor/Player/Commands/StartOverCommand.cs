using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class StartOverCommand : PlayerCommand<StartOverEvent>
  {
    protected override void Execute(StartOverEvent evt, PlayerContext context, LevelPlayer view)
    {
      view.Reset();

      view.TileGridControl.RemoveAllTiles();
      view.TileGridControl.RemoveAllEdges();
    }
  }
}
