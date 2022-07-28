using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class RandomChangeItemLevelSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var description = state.Configuration.Providers.SpellDescriptionProvider.Get(useSpell.Id);
      var levelDescription = description.GetLevel(useSpell.Level);

      var grid = state.TileGrid;

      ChangeItemEvent changeItemEvent = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        changeItemEvent = state.Output.EnqueueByFactory<ChangeItemEvent>(state.Tick);

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      var tiles = grid.Tiles.Where(t => !t.IsEmpty && t.ItemType == ItemType.Cell).OrderBy(t => state.GetNextRandom(grid.TileCount)).Take(levelDescription.Value).ToArray();
      if (tiles.Length != 0)
      {
        foreach (var tile in tiles)
        {
          var newItem = tile.Item.Copy();
          if (newItem.Level == LevelId.L0) newItem.Level = LevelId.L1;
          else newItem.Level = LevelId.L0;
          tile.SetNextItem(newItem);
          tile.ApplyNextItem();

          if (isGenerateOutputEvents)
          {
            useSpellActionEvent.ChangeTiles.Add(tile.Position);

            changeItemEvent.Queue.Enqueue(new ChangeItemEvent.Data
            {
              Item = newItem,
              Position = tile.Position
            });
          }
        }
      }
    }
  }
}
