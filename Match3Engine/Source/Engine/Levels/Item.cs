using System;

namespace Match3.Engine.Levels
{
  [Serializable]
  public class Item : IEquatable<Item>
  {
    public int Id;
    public int Level;

    public Item()
    {

    }

    public Item(int id, int level)
    {
      Id = id;
      Level = level;
    }

    public Item Copy()
    {
      return new Item
      {
        Id = Id,
        Level = Level
      };
    }

    public Item CopyWithId(int id)
    {
      return new Item
      {
        Id = id,
        Level = Level
      };
    }

    public Item Copy(int id, int level)
    {
      return new Item
      {
        Id = id,
        Level = level
      };
    }

    public Item CopyAndIncreaseLevel()
    {
      var copy = Copy();
      copy.Level += 1;
      return copy;
    }

    public override string ToString()
    {
      return string.Format("Item(id:{0}, level:{1})", Id, Level);
    }

    public bool Equals(Item other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Id == other.Id && Level == other.Level;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((Item)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Id * 397) ^ Level;
      }
    }
  }
}
