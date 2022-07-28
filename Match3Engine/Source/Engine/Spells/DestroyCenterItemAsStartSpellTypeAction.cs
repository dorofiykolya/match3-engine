using System;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Utils;

namespace Match3.Engine.Spells
{
  public class DestroyCenterItemAsStartSpellTypeAction : SpellTypeAction
  {
    private readonly Point[] _offsets;

    public DestroyCenterItemAsStartSpellTypeAction()
    {
      _offsets = PatternParser.Parse(@"[#| | | |#]" +
                                      "[ |#| |#| ]" +
                                      "[ | |X| | ]" +
                                      "[ |#| |#| ]" +
                                      "[#| | | |#]", true);
    }

    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var grid = state.TileGrid;
      var activator = state.TileGridActivator;

      var tile = grid.GetTile(useSpell.Positions[0]);
      if (tile == null) throw new ArgumentException("ячейка не существует");
      if (tile.IsEmpty) throw new ArgumentException("ячейка пуста");

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();

        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      activator.Activate(tile.Position, activationResult);

      foreach (var offset in _offsets)
      {
        var position = tile.Position + offset;

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
