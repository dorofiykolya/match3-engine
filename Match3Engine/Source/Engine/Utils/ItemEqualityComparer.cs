using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.Utils
{
  public class ItemEqualityComparer : IEqualityComparer<Item>
  {
    public static readonly ItemEqualityComparer Default = new ItemEqualityComparer();
    public static readonly ItemReferenceEqualityComparer Reference = new ItemReferenceEqualityComparer();

    public bool Equals(Item x, Item y)
    {
      return x.Equals(y);
    }

    public int GetHashCode(Item obj)
    {
      return obj.GetHashCode();
    }
  }

  public class ItemReferenceEqualityComparer : IEqualityComparer<Item>
  {
    public bool Equals(Item x, Item y)
    {
      var reference = ReferenceEquals(x, y);
      return reference;
    }

    public int GetHashCode(Item obj)
    {
      return ((object)obj).GetHashCode();
    }
  }
}
