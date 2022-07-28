namespace Match3.Engine.Levels
{
  public class Position
  {
    protected bool Equals(Position other)
    {
      return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((Position)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
      }
    }

    public Position(Point point) : this(point.X, point.Y)
    {

    }

    public Position(int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; private set; }
    public int Y { get; private set; }

    public override string ToString()
    {
      return string.Format("Position({0}:{1})", X, Y);
    }

    public static implicit operator Point(Position position)
    {
      return new Point(position.X, position.Y);
    }

    public static bool operator ==(Position l, Position r)
    {
      if (ReferenceEquals(l, r)) return true;
      if (ReferenceEquals(l, null)) return false;
      return l.Equals(r);
    }

    public static bool operator !=(Position l, Position r)
    {
      if (ReferenceEquals(l, r)) return false;
      if (ReferenceEquals(l, null)) return true;
      return !l.Equals(r);
    }
  }
}
