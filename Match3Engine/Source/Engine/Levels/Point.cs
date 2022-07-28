using System;

namespace Match3.Engine.Levels
{
  [Serializable]
  public struct Point : IEquatable<Point>, IComparable<Point>, ICloneable
  {
    public static readonly Point Zero = new Point(0, 0);

    public static readonly Point Left = new Point(-1, 0);
    public static readonly Point Right = new Point(1, 0);
    public static readonly Point Top = new Point(0, -1);
    public static readonly Point Bottom = new Point(0, 1);

    public static readonly Point[] Directions = { Left, Right, Top, Bottom };

    public static Point Direction(Direction direction)
    {
      switch (direction)
      {
        case Levels.Direction.Left:
          return Left;
        case Levels.Direction.Right:
          return Right;
        case Levels.Direction.Top:
          return Top;
        case Levels.Direction.Bottom:
          return Bottom;
      }
      return Zero;
    }

    public int X;
    public int Y;

    public Point(int x, int y)
    {
      X = x;
      Y = y;
    }

    public static double Distance(Point point1, Point point2)
    {
      var x = point1.X - point2.X;
      var y = point1.Y - point2.Y;
      if (x < 0) x = -x;
      if (y < 0) y = -y;
      return Math.Sqrt(x * x + y * y);
    }

    public static Point operator +(Point s1, Point s2)
    {
      return new Point(s1.X + s2.X, s1.Y + s2.Y);
    }

    public static Point operator +(Point s1, int s2)
    {
      return new Point(s1.X + s2, s1.Y + s2);
    }

    public static Point operator -(Point s1, Point s2)
    {
      return new Point(s1.X - s2.X, s1.Y - s2.Y);
    }

    public static Point operator -(Point s1, int s2)
    {
      return new Point(s1.X - s2, s1.Y - s2);
    }

    public static Point operator *(Point s1, Point s2)
    {
      return new Point(s1.X * s2.X, s1.Y * s2.Y);
    }

    public static Point operator *(Point s1, double s2)
    {
      return new Point((int)(s1.X * s2), (int)(s1.Y * s2));
    }

    public static Point operator *(Point s1, int s2)
    {
      return new Point(s1.X * s2, s1.Y * s2);
    }

    public static Point operator /(Point s1, Point s2)
    {
      return new Point(s1.X / s2.X, s1.Y / s2.Y);
    }

    public static Point operator /(Point s1, double s2)
    {
      return new Point((int)(s1.X / s2), (int)(s1.Y / s2));
    }

    public static Point operator /(Point s1, int s2)
    {
      return new Point(s1.X / s2, s1.Y / s2);
    }

    public static bool operator ==(Point s1, Point s2)
    {
      return s1.Equals(s2);
    }

    public static bool operator !=(Point s1, Point s2)
    {
      return !s1.Equals(s2);
    }

    public void SetTo(int x, int y)
    {
      X = x;
      Y = y;
    }

    public double GetLength()
    {
      return Math.Sqrt(X * X + Y * Y);
    }

    public bool Equals(Point other)
    {
      return other.X == X && other.Y == Y;
    }

    public bool Equals(int x, int y)
    {
      return X == x && X == y;
    }

    public override bool Equals(object other)
    {
      if (ReferenceEquals(null, other)) return false;
      return other is Point && Equals((Point)other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
      }
    }

    public int CompareTo(Point other)
    {
      if (GetLength() > other.GetLength()) return 1;
      return -1;
    }

    public override string ToString()
    {
      return string.Format("[Point(x:{0}, y:{1})]", X, Y);
    }

    /// <summary>
    /// format
    /// </summary>
    /// <param name="format">"xCoord:{x}, yCoord:{y}"</param>
    /// <returns></returns>
    public string ToString(string format)
    {
      return format.Replace("{x}", X.ToString()).Replace("{y}", Y.ToString());
    }

    public object Clone()
    {
      return new Point(X, Y);
    }

    public Point Copy()
    {
      return new Point(X, Y);
    }
  }
}
