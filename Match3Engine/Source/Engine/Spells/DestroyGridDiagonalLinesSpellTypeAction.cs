using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class DestroyGridDiagonalLinesSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var grid = state.TileGrid;
      var activator = state.TileGridActivator;
      var bounds = grid.Bounds;

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      var x = bounds.MinX;
      var y = bounds.MinY;
      for (; x <= bounds.MaxX; x++, y++)
      {
        var firstPosition = new Point(x, y);
        var secondPosition = new Point(x, bounds.MaxY - y);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(firstPosition);
          useSpellActionEvent.ActivateTiles.Add(secondPosition);
        }

        activator.Activate(firstPosition, activationResult);
        activator.Activate(secondPosition, activationResult);
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
