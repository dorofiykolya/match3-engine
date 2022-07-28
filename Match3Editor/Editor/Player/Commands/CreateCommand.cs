using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  class CreateCommand : PlayerCommand<CreateEvent>
  {
    protected override void Execute(CreateEvent evt, PlayerContext context, LevelPlayer view)
    {
      view.SetRequirements(evt.Requirements);
      view.SetSwaps(0, evt.Swaps);
      view.SetEnergy(evt.Energy);
      view.SetScore(evt.Score);
      
      foreach (var tileInfo in evt.Tiles)
      {
        view.TileGridControl.AddTile(tileInfo);
      }

      foreach (var edgeInfo in evt.Edges)
      {
        view.TileGridControl.AddEdge(edgeInfo);
      }
    }
  }
}
