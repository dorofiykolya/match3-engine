using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Providers;
using Match3.Engine.Shareds.Providers;
using Match3.Settings;

namespace Match3.Providers
{
  public class Match3ProviderFactory
  {
    public IEngineProviders Create(Match3Setting setting)
    {
      SharedProviders providers = new SharedProviders
      {
        ItemsProvider = new SharedItemDescriptionProvider(setting.Items),
        CombinationActivatorsProvider = new SharedCombinationActivatorsProvider(),
        ModifierDescriptionProvider = new SharedModifierDescriptionProvider(setting.Modifiers),
        MatchesProvider = new SharedMatchesProvider(),
        CommandsProvider = new SharedCommandsProvider(),
        SpellTypeActionsProvider = new SharedSpellTypeActionProvider(),
        SpellCombinationsProvider = new SharedSpellCombinationsProvider(setting.SpellCombinations),
        SpellDescriptionProvider = new SharedSpellDescriptionProvider(setting.Spells),
        ModulesProvider = new SharedModulesProvider()
      };

      return providers;
    }
  }
}
