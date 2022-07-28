using System;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// меняет выбранные ячейки местами
  /// </summary>
  public class SwapItemSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      if (useSpell.Positions[0] == useSpell.Positions[1]) throw new ArgumentException("нельзя менять цвет у одной и той же ячейки");

      var grid = state.TileGrid;
      var firstTile = grid.GetTile(useSpell.Positions[0]);
      if (firstTile == null) throw new ArgumentException("не найдена ячейка по координатам useSpell.Position:" + useSpell.Positions[0]);

      var secondTile = grid.GetTile(useSpell.Positions[1]);
      if (secondTile == null) throw new ArgumentException("не найдена ячейка по координатам useSpell.SecondPosition:" + useSpell.Positions[1]);

      var firstItem = firstTile.Item;
      if (firstItem == null) throw new ArgumentException("нельзя менять местами ячейку с пустой ячейкой");
      if (firstTile.ItemType != ItemType.Cell) throw new ArgumentException("выбраная ячейка должна быть типа ItemType.Cell");

      var secondItem = secondTile.Item;
      if (secondItem == null) throw new ArgumentException("нельзя менять местами ячейку с пустой ячейкой");
      if (secondTile.ItemType != ItemType.Cell) throw new ArgumentException("выбраная ячейка должна быть типа ItemType.Cell");


      firstTile.SetNextItem(secondItem);
      secondTile.SetNextItem(firstItem);

      firstTile.ApplyNextItem();
      secondTile.ApplyNextItem();

      if (state.Environment.IsGenerateOutputEvents())
      {
        var useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
        useSpellActionEvent.ChangeTiles.Add(firstTile.Position);
        useSpellActionEvent.ChangeTiles.Add(secondTile.Position);

        var changeItemEvent = state.Output.EnqueueByFactory<ChangeItemEvent>(state.Tick);
        changeItemEvent.Queue.Enqueue(new ChangeItemEvent.Data
        {
          Item = firstTile.Item,
          Position = firstTile.Position
        });
        changeItemEvent.Queue.Enqueue(new ChangeItemEvent.Data
        {
          Item = secondTile.Item,
          Position = secondTile.Position
        });
      }
    }
  }
}
