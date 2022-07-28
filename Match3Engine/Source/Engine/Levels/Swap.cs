using System;
using System.Reflection;

namespace Match3.Engine.Levels
{
  public class Swap : IEquatable<Swap>
  {
    public Swap(Point first, Point second)
    {
      //if (first.X < 0 || first.Y < 0) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": first.X < 0 || first.Y < 0, first:{0}, second:{1}", first, second));
      //if (second.X < 0 || second.Y < 0) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": second.X < 0 || second.Y < 0, first:{0}, second:{1}", first, second));
      if (first == second) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": нельзя переместить ячейки с одинаковыми координатами, first:{0}, second:{1}", first, second));

      var x = Math.Abs(first.X - second.X);
      var y = Math.Abs(first.Y - second.Y);

      if (!(x == 0 && y == 1 || x == 1 && y == 0))
      {
        throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": не коректные координаты ячеек для перемещения, first:{0}, second:{1}", first, second));
      }

      First = first;
      Second = second;
    }

    public Point First { get; private set; }
    public Point Second { get; private set; }

    public Orientation Orientation { get { return Math.Abs(First.X - Second.X) > 0 ? Orientation.Horizontal : Orientation.Vertical; } }
    public Direction FirstDirection { get { return Orientation == Orientation.Horizontal ? (First.X - Second.X > 0 ? Direction.Left : Direction.Right) : (First.Y - Second.Y > 0 ? Direction.Top : Direction.Bottom); } }
    public Direction SecondDirection { get { return FirstDirection.Invert(); } }

    public static bool IsValid(Point first, Point second)
    {
      if (first == second) return false;

      var x = Math.Abs(first.X - second.X);
      var y = Math.Abs(first.Y - second.Y);

      if (!(x == 0 && y == 1 || x == 1 && y == 0))
      {
        return false;
      }

      return true;
    }

    public static bool operator ==(Swap left, Swap right)
    {
      if (ReferenceEquals(left, right)) return true;
      if (ReferenceEquals(left, null)) return false;
      if (ReferenceEquals(right, null)) return false;
      return left.Equals(right);
    }

    public static bool operator !=(Swap left, Swap right)
    {
      if (ReferenceEquals(left, right)) return false;
      if (ReferenceEquals(left, null)) return true;
      if (ReferenceEquals(right, null)) return true;
      return !left.Equals(right);
    }

    public bool Equals(Swap other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return First.Equals(other.First) && Second.Equals(other.Second) || First.Equals(other.Second) && Second.Equals(other.First);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((Swap)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (First.GetHashCode() * 397) ^ Second.GetHashCode();
      }
    }

    public override string ToString()
    {
      return string.Format("[Swap(First:{0}, Second:{1})]", First, Second);
    }
  }
}
