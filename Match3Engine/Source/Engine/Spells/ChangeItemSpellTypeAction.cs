using System;
using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// Шлем – перекрашивание камней. Некоторое количество камней меняет свой цвет на какой-то определённо выбранный.
  /// </summary>
  public class ChangeItemSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var position = useSpell.Positions[0];
      var selectedTile = state.GridProvider.GetTile(position);
      if (selectedTile == null) throw new ArgumentException("ячека не может не существовать");
      if (selectedTile.IsEmpty) throw new ArgumentException("ячека не может быть пуста");
      if (selectedTile.ItemType != ItemType.Cell) throw new ArgumentException("ячека должна быть типа ItemType.Cell, текущий:" + selectedTile.ItemType);

      var providers = state.Configuration.Providers;
      var spell = providers.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);
      var count = spellLevel.Value;
      var tiles = state.TileGrid.Tiles.Where(tile => !tile.IsEmpty && tile != selectedTile && tile.Item.Id != selectedTile.Item.Id && tile.ItemType == ItemType.Cell).ToArray();
      var tilesCount = tiles.Length;
      while (--tilesCount > 0)
      {
        var index = state.GetNextRandom(tilesCount);
        var current = tiles[tilesCount];
        var next = tiles[index];
        tiles[tilesCount] = next;
        tiles[index] = current;
      }

      ChangeItemEvent changeItemEvent = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (state.Configuration.Environment.IsGenerateOutputEvents())
      {
        changeItemEvent = state.Output.EnqueueByFactory<ChangeItemEvent>(state.Tick);
        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      var newItemId = selectedTile.Item.Id;

      for (int i = 0; i < Math.Min(count, tiles.Length); i++)
      {
        var tile = tiles[i];
        //var newId = GetRandomItemId(state, availableItems, tile.Item.Id);
        var item = tile.Item.Copy();
        item.Id = newItemId;
        tile.SetNextItem(item);
        tile.ApplyNextItem();

        if (useSpellActionEvent != null)
        {
          useSpellActionEvent.ChangeTiles.Add(tile.Position);
        }
        if (changeItemEvent != null)
        {
          changeItemEvent.Queue.Enqueue(new ChangeItemEvent.Data
          {
            Item = item.Copy(),
            Position = tile.Position
          });
        }
      }
    }

    private int GetRandomItemId(IEngineNextRandom state, int[] available, int excludeId)
    {
      var index = state.GetNextRandom(available.Length);
      if (available[index] == excludeId)
      {
        ++index;
        if (index >= available.Length) index = 0;
      }
      return available[index];
    }
  }
}
