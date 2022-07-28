using System;
using System.Linq;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// Кираса – создание бонусов. Несколько камней становятся бонусными.
  /// </summary>
  public class MakeBonusItemSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var providers = state.Configuration.Providers;
      var spell = providers.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);
      var count = spellLevel.Value;

      var tiles = state.TileGrid.Tiles.Where(tile => !tile.IsEmpty && tile.Item.Level < LevelId.L1).ToArray();
      var tilesCount = tiles.Length;
      while (--tilesCount > 0)
      {
        var index = state.GetNextRandom(tilesCount);
        var current = tiles[tilesCount];
        var next = tiles[index];
        tiles[tilesCount] = next;
        tiles[index] = current;
      }

      var isGenerateOutputEvents = state.Configuration.Environment.IsGenerateOutputEvents();

      ChangeItemEvent changeItemEvent = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        changeItemEvent = state.Output.EnqueueByFactory<ChangeItemEvent>(state.Tick);

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      for (int i = 0; i < Math.Min(count, tiles.Length); i++)
      {
        var tile = tiles[i];
        tile.SetNextItem(tile.Item.CopyAndIncreaseLevel());
        tile.ApplyNextItem();

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ChangeTiles.Add(tile.Position);

          changeItemEvent.Queue.Enqueue(new ChangeItemEvent.Data
          {
            Position = tile.Position,
            Item = tile.Item.Copy()
          });
        }
      }
    }
  }
}
