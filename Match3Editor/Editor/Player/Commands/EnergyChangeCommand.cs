using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class EnergyChangeCommand : PlayerCommand<EnergyChangeEvent>
  {
    protected override void Execute(EnergyChangeEvent evt, PlayerContext context, LevelPlayer view)
    {
      view.SetEnergy(evt.Energy);
    }
  }
}
