using Match3.Engine.Descriptions.SpellCombinations;

namespace Match3.Engine.Providers
{
  public interface ISpellCombinationsProvider
  {
    SpellCombinationDescription[] Collection { get; }
  }
}
