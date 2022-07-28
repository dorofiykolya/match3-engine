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
  public class FinishCommand : PlayerCommand<FinishEvent>
  {
    protected override void Execute(FinishEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Finish(evt, context, view));
    }

    private IEnumerator Finish(FinishEvent evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;
      view.Finish(evt.Reason);
    }
  }
}
