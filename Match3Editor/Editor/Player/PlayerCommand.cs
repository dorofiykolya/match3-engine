using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player
{
  public abstract class PlayerCommand<T> : PlayerCommand where T : OutputEvent
  {
    public override void Execute(OutputEvent evt, PlayerContext context, LevelPlayer view)
    {
      Execute((T)evt, context, view);
    }

    protected abstract void Execute(T evt, PlayerContext context, LevelPlayer view);
  }

  public abstract class PlayerCommand
  {
    public abstract void Execute(OutputEvent evt, PlayerContext context, LevelPlayer view);
  }
}
