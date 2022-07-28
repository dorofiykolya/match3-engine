using System.Collections.Generic;
using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Providers;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedSpellDescriptionProvider : ISpellDescriptionProvider
  {
    private readonly SpellDescription[] _spells;
    private readonly Dictionary<int, SpellDescription> _map;

    public SharedSpellDescriptionProvider(SpellDescription[] spells)
    {
      _spells = spells;
      _map = new Dictionary<int, SpellDescription>();

      foreach (var spell in spells)
      {
        _map[spell.Id] = spell;
      }
    }

    public SpellDescription Get(int id)
    {
      return _map[id];
    }

    public SpellDescription[] Collection
    {
      get { return _spells; }
    }
  }
}
