using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class DestoryVerticalLineThroughOneSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();
        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      var grid = state.TileGrid;
      var activator = state.TileGridActivator;
      var bounds = grid.Bounds;

      for (int x = bounds.MinX; x <= bounds.MaxX; x += 2)
      {
        for (int y = bounds.MinY; y <= bounds.MaxY; y++)
        {
          var position = new Point(x, y);

          if (isGenerateOutputEvents)
          {
            useSpellActionEvent.ActivateTiles.Add(position);
          }

          activator.Activate(position, activationResult);
        }
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
