using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Spells;

namespace Match3.Engine.Providers
{
  public interface ISpellTypeActionsProvider
  {
    SpellTypeAction Get(SpellType spellType);
  }
}
