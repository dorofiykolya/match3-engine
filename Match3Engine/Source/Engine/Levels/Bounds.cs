using System;

namespace Match3.Engine.Levels
{
  public struct Bounds
  {
    public int MinX;
    public int MinY;
    public int MaxX;
    public int MaxY;

    public int Width { get { return Math.Abs(MaxX + 1 - MinX); } }
    public int Height { get { return Math.Abs(MaxY + 1 - MinY); } }

    public bool Contains(Point point)
    {
      return MinX <= point.X &&
             MaxX >= point.X &&
             MinY <= point.Y &&
             MaxY >= point.Y;
    }
  }
}
