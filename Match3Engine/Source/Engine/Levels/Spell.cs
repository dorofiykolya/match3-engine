using System;

namespace Match3.Engine.Levels
{
  public class Spell : IComparable<Spell>, IComparable, IEquatable<Spell>
  {
    public int Id;
    public int Level;
    public int Count;

    public int CompareTo(Spell other)
    {
      if (ReferenceEquals(this, other)) return 0;
      if (ReferenceEquals(null, other)) return 1;
      var idComparison = Id.CompareTo(other.Id);
      if (idComparison != 0) return idComparison;
      var levelComparison = Level.CompareTo(other.Level);
      if (levelComparison != 0) return levelComparison;
      return Count.CompareTo(other.Count);
    }

    public int CompareTo(object obj)
    {
      return CompareTo(obj as Spell);
    }

    public bool Equals(Spell other)
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
      return Equals((Spell)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Id * 397) ^ Level;
      }
    }

    public override string ToString()
    {
      return string.Format("[Spell(Id:{0}, Level:{1}, Count:{2})]", Id, Level, Count);
    }
  }
}
