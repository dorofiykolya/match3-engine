using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class RandomMakeBonusItemAndActivateSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();
      var description = state.Configuration.Providers.SpellDescriptionProvider.Get(useSpell.Id);
      var levelDescription = description.GetLevel(useSpell.Level);

      var tiles = state.TileGrid.Tiles.Where(t => !t.IsEmpty && t.ItemType == ItemType.Cell && t.Item.Level == LevelId.L0)
        .OrderBy(a => state.GetNextRandom(state.TileGrid.TileCount)).Take(levelDescription.Value).ToArray();

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      foreach (var tile in tiles)
      {
        tile.SetNextItem(tile.Item.CopyAndIncreaseLevel());
        tile.ApplyNextItem();

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ChangeTiles.Add(tile.Position);

          var evt = state.Output.EnqueueByFactory<IncreaseLevelEvent>(state.Tick);
          evt.Items.Add(new IncreaseLevelEvent.IncreaseItem
          {
            Item = tile.Item,
            Position = tile.Position
          });
        }
      }

      var activator = state.TileGridActivator;
      foreach (var tile in tiles)
      {
        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(tile.Position);
        }

        activator.Activate(tile.Position, activationResult);
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
