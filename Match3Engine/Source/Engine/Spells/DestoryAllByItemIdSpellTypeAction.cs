using System;
using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class DestoryAllByItemIdSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var grid = state.TileGrid;
      var tile = grid.GetTile(useSpell.Positions[0]);
      if (tile == null) throw new ArgumentException("ячейка не существует");
      if (tile.IsEmpty) throw new ArgumentException("ячейка пуста");
      if (tile.ItemType == ItemType.Artifact) throw new ArgumentException("не может применяться к артефакту");

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;

      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();
        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      var activator = state.TileGridActivator;
      var tiles = grid.Tiles.Where(t => !t.IsEmpty && t.Item.Id == tile.Item.Id);
      foreach (var activateTile in tiles)
      {
        if (useSpellActionEvent != null)
        {
          useSpellActionEvent.ActivateTiles.Add(activateTile.Position);
        }

        activator.Activate(activateTile.Position, activationResult);
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
