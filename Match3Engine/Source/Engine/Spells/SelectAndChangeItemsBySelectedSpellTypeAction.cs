using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class SelectAndChangeItemsBySelectedSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var description = state.Configuration.Providers.SpellDescriptionProvider.Get(useSpell.Id);
      var levelDescription = description.GetLevel(useSpell.Level);

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      ActivateEvent activateEvent = null;
      ChangeItemEvent changeItemEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;

        activateEvent = state.Output.EnqueueByFactory<ActivateEvent>(state.Tick);
        changeItemEvent = state.Output.EnqueueByFactory<ChangeItemEvent>(state.Tick);
      }

      var activator = state.TileGridActivator;
      var grid = state.TileGrid;
      var tiles = grid.Tiles.Where(t => !t.IsEmpty && t.ItemType == ItemType.Cell).ToList();
      if (tiles.Count != 0)
      {
        var tile = tiles[state.GetNextRandom(tiles.Count)];
        tiles.Remove(tile);

        activator.Activate(tile.Position, activationResult);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(tile.Position);

          activateEvent.InitializeFrom(activationResult.Queue);
        }

        var list =
          tiles.Where(t => t.Item.Id != tile.Item.Id)
            .OrderBy(s => state.GetNextRandom(tiles.Count))
            .Take(state.GetNextRandom(levelDescription.MinValue, levelDescription.Value))
            .ToArray();
        foreach (var currentTile in list)
        {
          var item = currentTile.Item.CopyWithId(tile.Item.Id);
          currentTile.SetNextItem(item);
          currentTile.ApplyNextItem();

          if (isGenerateOutputEvents)
          {
            useSpellActionEvent.ChangeTiles.Add(currentTile.Position);

            changeItemEvent.Queue.Enqueue(new ChangeItemEvent.Data
            {
              Item = item.Copy(),
              Position = currentTile.Position
            });
          }
        }
      }
      else
      {
        tiles = grid.Tiles.Where(t => !t.IsEmpty && t.ItemType == ItemType.UniversalSwapCell).ToList();
        var tile = tiles[state.GetNextRandom(tiles.Count)];

        activator.Activate(tile.Position, activationResult);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(tile.Position);

          activateEvent.InitializeFrom(activationResult.Queue);
        }
      }
    }
  }
}
