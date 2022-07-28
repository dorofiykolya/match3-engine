using System;
using System.Collections.Generic;
using Match3.Engine.CombinationActivators;
using Match3.Engine.Levels;
using Match3.Engine.Providers;
using Match3.Engine.Utils;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedCombinationActivatorsProvider : ICombinationActivatorsProvider
  {
    private readonly Dictionary<Map, SwapCombinationActivator> _mapBySwap = new Dictionary<Map, SwapCombinationActivator>();
    private readonly Dictionary<Item, CombinationActivator> _mapActivators = new Dictionary<Item, CombinationActivator>(ItemEqualityComparer.Default);

    public SharedCombinationActivatorsProvider()
    {
      var universalSwapItemActivator = new UniversalSwapItemActivator();
      
      _mapActivators[new Item(ItemId.Universal, LevelId.L0)] = universalSwapItemActivator;
    }

    public SwapCombinationActivator GetSwapActivator(Item from, Item to)
    {
      var key = GetKey(from, to);
      SwapCombinationActivator activator;
      if (_mapBySwap.TryGetValue(key, out activator))
      {
        return activator;
      }
      return null;
    }

    public CombinationActivator GetActivator(Item item)
    {
      CombinationActivator activator;
      if (_mapActivators.TryGetValue(item, out activator))
      {
        return activator;
      }
      return null;
    }

    private Map GetKey(Item from, Item to)
    {
      return new Map
      {
        FromId = from.Id,
        ToId = to.Id,
        FromLevel = from.Level,
        ToLevel = to.Level
      };
    }

    private Map GetKey(int fromId, int fromLevel, int toId, int toLevel)
    {
      return new Map
      {
        FromId = fromId,
        ToId = toId,
        FromLevel = fromLevel,
        ToLevel = toLevel
      };
    }

    private struct Map : IEquatable<Map>
    {
      public int FromId;
      public int FromLevel;
      public int ToId;
      public int ToLevel;

      public bool Equals(Map other)
      {
        return FromId == other.FromId && FromLevel == other.FromLevel && ToId == other.ToId && ToLevel == other.ToLevel;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        return obj is Map && Equals((Map)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          var hashCode = FromId;
          hashCode = (hashCode * 397) ^ FromLevel;
          hashCode = (hashCode * 397) ^ ToId;
          hashCode = (hashCode * 397) ^ ToLevel;
          return hashCode;
        }
      }

      public override string ToString()
      {
        return string.Format("from({0}:{1}), to({2}:{3})", FromId, FromLevel, ToId, ToLevel);
      }
    }
  }
}
