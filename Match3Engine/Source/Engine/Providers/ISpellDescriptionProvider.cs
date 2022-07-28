using Match3.Engine.Descriptions.Spells;

namespace Match3.Engine.Providers
{
  public interface ISpellDescriptionProvider
  {
    SpellDescription Get(int id);
    SpellDescription[] Collection { get; }
  }
}
