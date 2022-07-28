using Match3.Engine.Descriptions.SpellCombinations;
using Match3.Engine.Providers;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedSpellCombinationsProvider : ISpellCombinationsProvider
  {
    private readonly SpellCombinationDescription[] _combinations;

    public SharedSpellCombinationsProvider(SpellCombinationDescription[] combinations)
    {
      _combinations = combinations;
    }

    public SpellCombinationDescription[] Collection { get { return _combinations; } }
  }
}
