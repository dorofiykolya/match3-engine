using Match3.Engine.Providers;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedProviders : IEngineProviders
  {
    public IItemDescriptionProvider ItemsProvider { get; set; }
    public ILevelDescriptionProvider LevelsProvider { get; set; }
    public IModulesProvider ModulesProvider { get; set; }
    public ICommandsProvider CommandsProvider { get; set; }
    public IMatchesProvider MatchesProvider { get; set; }
    public ICombinationActivatorsProvider CombinationActivatorsProvider { get; set; }
    public IModifierDescriptionProvider ModifierDescriptionProvider { get; set; }
    public ISpellDescriptionProvider SpellDescriptionProvider { get; set; }
    public ISpellTypeActionsProvider SpellTypeActionsProvider { get; set; }
    public ISpellCombinationsProvider SpellCombinationsProvider { get; set; }
  }
}
