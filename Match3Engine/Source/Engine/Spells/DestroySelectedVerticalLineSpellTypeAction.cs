using System;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class DestroySelectedVerticalLineSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var grid = state.TileGrid;
      var tile = grid.GetTile(useSpell.Positions[0]);
      if (tile == null) throw new ArgumentException(GetType().Name + ": ячейка не существует: " + useSpell.Positions[0]);

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      var activator = state.TileGridActivator;
      var x = tile.Position.X;
      for (int y = grid.Bounds.MinY; y <= grid.Bounds.MaxY; y++)
      {
        var position = new Point(x, y);
        var currentTile = grid.GetTile(position);
        if (currentTile != null && !currentTile.IsEmpty && (currentTile.ItemType == ItemType.Cell || currentTile.ItemType == ItemType.UniversalSwapCell))
        {
          if (isGenerateOutputEvents)
          {
            useSpellActionEvent.ActivateTiles.Add(position);
          }

          activator.Activate(currentTile.Position, activationResult);
        }
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
