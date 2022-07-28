using System;
using System.Linq;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// Оружие – точечно рандомно разбивает несколько камней на поле. От уровня растёт количество разбиваемых камней.
  /// </summary>
  public class RandomDestorySpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var providers = state.Configuration.Providers;
      var spell = providers.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);
      var count = spellLevel.Value;
      var activator = state.TileGridActivator;

      var tiles = state.TileGrid.Tiles.Where(tile => !tile.IsEmpty).ToArray();
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

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();
        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      for (int i = 0; i < Math.Min(count, tiles.Length); i++)
      {
        var tile = tiles[i];

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(tile.Position);
        }

        activator.Activate(tile.Position, activationResult);
      }

      if (isGenerateOutputEvents)
      {
        var activateEvent = state.Output.EnqueueByFactory<ActivateEvent>(state.Tick);
        activateEvent.InitializeFrom(activationResult.Queue);
      }
    }
  }
}
