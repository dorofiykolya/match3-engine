using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Utils;

namespace Match3.Engine.Spells
{
  /// <summary>
  /// Щит – разбивает камни по определённой площади. Площадь поражения растёт от уровня.
  /// </summary>
  public class SplashSpellTypeAction : SpellTypeAction
  {
    private readonly Point[][] _patternByLevel;

    public SplashSpellTypeAction()
    {
      _patternByLevel = new[]
      {
        PatternParser.Parse(@"[X]", true),

        PatternParser.Parse(@"[#|X|#]", true),

        PatternParser.Parse(@"[ |#| ]"+
                             "[#|X|#]" +
                             "[ |#| ]", true),

        PatternParser.Parse(@"[#|#|#]"+
                             "[#|X|#]" +
                             "[#|#|#]", true),

        PatternParser.Parse(@"[#|#|#|#|#]" +
                             "[#|#|X|#|#]" +
                             "[#|#|#|#|#]", true),

        PatternParser.Parse(@"[ |#|#|#| ]"+
                             "[#|#|#|#|#]" +
                             "[#|#|X|#|#]" +
                             "[#|#|#|#|#]" +
                             "[ |#|#|#| ]", true),

        PatternParser.Parse(@"[#|#|#|#|#]"+
                             "[#|#|#|#|#]" +
                             "[#|#|X|#|#]" +
                             "[#|#|#|#|#]" +
                             "[#|#|#|#|#]", true),
      };
    }

    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var providers = state.Configuration.Providers;
      var spell = providers.SpellDescriptionProvider.Get(useSpell.Id);
      var spellLevel = spell.GetLevel(useSpell.Level);
      var count = spellLevel.Value;
      var activator = state.TileGridActivator;

      var position = useSpell.Positions[0];
      var offsets = _patternByLevel[useSpell.Level];

      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();
        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      foreach (var offset in offsets)
      {
        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(position + offset);
        }
        activator.Activate(position + offset, activationResult);
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
