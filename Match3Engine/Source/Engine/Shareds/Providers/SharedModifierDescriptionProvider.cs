using System.Collections.Generic;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Providers;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedModifierDescriptionProvider : IModifierDescriptionProvider
  {
    private readonly Dictionary<int, ModifierDescription> _map;

    public SharedModifierDescriptionProvider(ModifierDescription[] modifiers)
    {
      _map = new Dictionary<int, ModifierDescription>();

      foreach (var modifier in modifiers)
      {
        _map[modifier.Id] = modifier;
      }
    }

    public ModifierDescription Get(int id)
    {
      return _map[id];
    }
  }
}
