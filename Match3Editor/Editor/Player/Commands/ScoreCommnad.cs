using System.Collections;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class ScoreCommnad : PlayerCommand<ScoreEvent>
  {
    protected override void Execute(ScoreEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Score(view, evt.Score));
    }

    private IEnumerator Score(LevelPlayer view, int score)
    {
      yield return null;
      view.SetScore(score);
    }
  }
}
