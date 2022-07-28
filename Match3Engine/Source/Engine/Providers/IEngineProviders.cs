namespace Match3.Engine.Providers
{
  public interface IEngineProviders
  {
    IItemDescriptionProvider ItemsProvider { get; }
    IModulesProvider ModulesProvider { get; }
    ICommandsProvider CommandsProvider { get; }
    IMatchesProvider MatchesProvider { get; }
    ICombinationActivatorsProvider CombinationActivatorsProvider { get; }
    IModifierDescriptionProvider ModifierDescriptionProvider { get; }
    ISpellDescriptionProvider SpellDescriptionProvider { get; }
    ISpellTypeActionsProvider SpellTypeActionsProvider { get; }
    ISpellCombinationsProvider SpellCombinationsProvider { get; set; }
  }
}
