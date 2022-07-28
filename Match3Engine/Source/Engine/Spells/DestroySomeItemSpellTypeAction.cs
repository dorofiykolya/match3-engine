using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// Сапоги – уничтожает камни выбранного цвета. Целью выбирается камень. Затем начиная от 12 часов по часовой стрелки начинают искать камни того же цвета и уничтожаться, пока не достигнут лимита заклинаний. С уровнем растёт количество камней, которые будут уничтожены.
  /// </summary>
  public class DestroySomeItemSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var providers = state.Configuration.Providers;
      var spell = providers.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);
      var count = spellLevel.Value;
      var activator = state.TileGridActivator;

      var tile = state.TileGrid.GetTile(useSpell.Positions[0]);
      if (tile == null || tile.IsEmpty) throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + string.Format("невозможно использовать спелл, не верно заданы координаты ячейки, ячейка должна существовать и не может быть пуста, Spell(id:{0}, level:{1}, position:{2})", useSpell.Id, useSpell.Level, useSpell.Positions[0]));

      var isGenerateOutputEvents = state.Configuration.Environment.IsGenerateOutputEvents();

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;

      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
        useSpellActionEvent.ActivateTiles.Add(tile.Position);
      }

      activator.Activate(tile.Position, activationResult);

      var itemId = tile.Item.Id;
      var tiles = state.TileGrid.Tiles.Where(i => !i.IsEmpty && i != tile && i.Item.Id == itemId);
      foreach (var sortedTile in Sort(tiles, tile).Take(count))
      {
        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(sortedTile.Position);
        }

        activator.Activate(sortedTile.Position, activationResult);
      }

      if (activationResult != null)
      {
        var evt = state.Output.EnqueueByFactory<ActivateEvent>(state.Tick);
        evt.InitializeFrom(activationResult.Queue);
      }
    }

    private IEnumerable<Tile> Sort(IEnumerable<Tile> collection, Tile pivot)
    {
      return collection.OrderBy(t => GetGradus(pivot.Position, t.Position));
    }

    private double GetGradus(Point fromTile, Point toTile)
    {
      var angle = Math.Atan2(fromTile.Y - toTile.Y, fromTile.X - toTile.Y);
      return (90 + ((angle * (180 / Math.PI)) + 180)) % 360;
    }
  }
}
