using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Utils;

namespace Match3.Engine.Spells
{
  public class RandomSplashByValueSpellTypeAction : SpellTypeAction
  {
    private readonly Point[] _offsets;

    public RandomSplashByValueSpellTypeAction()
    {
      _offsets = PatternParser.Parse(@"[#|#|#]" +
                                      "[#|X|#]" +
                                      "[#|#|#]", true);
    }

    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var grid = state.TileGrid;
      var tiles = grid.Tiles.Where(t => !t.IsEmpty && (t.ItemType == ItemType.Cell || t.ItemType == ItemType.UniversalSwapCell)).ToArray();
      Tile tile = null;
      if (tiles.Length != 0)
      {
        tile = tiles[state.GetNextRandom(tiles.Length)];
      }

      if (tile != null)
      {
        var activator = state.TileGridActivator;

        ActivationResult activationResult = null;
        UseSpellActionEvent useSpellActionEvent = null;
        if (isGenerateOutputEvents)
        {
          activationResult = new ActivationResult();

          useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
          useSpellActionEvent.UseSpell = useSpell;
          useSpellActionEvent.ActivateTiles.Add(useSpell.Positions[0]);
        }

        activator.Activate(useSpell.Positions[0], activationResult);

        foreach (var offset in _offsets)
        {
          var position = useSpell.Positions[0] + offset;
          if (isGenerateOutputEvents)
          {
            useSpellActionEvent.ActivateTiles.Add(position);
          }
          activator.Activate(position, activationResult);
        }

        if (isGenerateOutputEvents)
        {
          state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
        }
      }
    }
  }
}
