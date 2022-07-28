using System;
using System.Reflection;
using Match3.Engine.Utils;

namespace Match3.Engine.Levels
{
  public class PositionConverter
  {
    public Point TileToEdge(Point position, Direction direction)
    {
      if (position.X < 0 || position.Y < 0) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": позиция не может быть отрицательной, position:" + position);

      var edge = new Point(TileToEdge(position.X), TileToEdge(position.Y)) + Point.Direction(direction);
      return edge;
    }

    public Point EdgeToTile(Point position, Direction direction)
    {
      if (position.X < 0 || position.Y < 0) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": позиция не может быть отрицательной, position:" + position);

      var tile = (position - 1 + Point.Direction(direction)) / 2;
      return tile;
    }

    public Orientation GetEdgeOrientationByPosition(Point edgePosition)
    {
      if (edgePosition.X % 2 == 0 && edgePosition.Y % 2 == 0) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": позиция не соответствует правильной ориентации, edgePosition:" + edgePosition);
      if (edgePosition.X % 2 == 1 && edgePosition.Y % 2 == 1) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": позиция не соответствует правильной ориентации, edgePosition:" + edgePosition);

      if (edgePosition.X % 2 == 0) return Orientation.Horizontal;
      return Orientation.Vertical;
    }

    public Direction GetEdgeDirectionByTile(Point edgePosition, Point tilePosition)
    {
      var tX = TileToEdge(tilePosition.X);
      var tY = TileToEdge(tilePosition.Y);

      var x = tX - edgePosition.X;
      var y = tY - edgePosition.Y;

      if (Math.Abs(x) > 1 || Math.Abs(y) > 1) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": заданы не правильные координаты границы относительно ячейки, edgePosition:{0}, tilePosition:{1}", edgePosition, tilePosition));
      if (x == 0 && y == 0) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": заданы не правильные координаты границы относительно ячейки, edgePosition:{0}, tilePosition:{1}", edgePosition, tilePosition));
      if (Math.Abs(x) == 1 && Math.Abs(y) == 1) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + string.Format(": заданы не правильные координаты границы относительно ячейки, edgePosition:{0}, tilePosition:{1}", edgePosition, tilePosition));

      if (x > 0) return Direction.Right;
      if (x < 0) return Direction.Left;
      if (y > 0) return Direction.Bottom;
      if (y < 0) return Direction.Top;

      return Direction.Bottom;
    }

    public Point TileToEdgeInternal(Point value)
    {
      return value * 2 + 1;
    }

    private static int TileToEdge(int value)
    {
      return value * 2 + 1;
    }
  }
}
