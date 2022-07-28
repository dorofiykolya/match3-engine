using System.Collections.Generic;
using Match3.Engine.Descriptions.Modifiers;

namespace Match3.Engine.Levels
{
  public class ModifierActivateData : Dictionary<Tile, ModifierActivatorType>
  {
    public void Set(Tile tile, ModifierActivatorType type)
    {
      ModifierActivatorType value;
      TryGetValue(tile, out value);
      this[tile] = value | type;
    }
  }
}
