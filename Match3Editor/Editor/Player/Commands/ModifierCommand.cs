using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class ModifierCommand : PlayerCommand<ModifierEvent>
  {
    protected override void Execute(ModifierEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Do(evt, context, view));
    }

    private IEnumerator Do(ModifierEvent evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;

      var tile = view.TileGridControl.GetTile(evt.Position);
      tile.Modifier.Change(evt.Id, evt.Level);
    }
  }
}
