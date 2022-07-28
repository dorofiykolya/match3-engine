using System;

namespace Match3.Engine.Levels
{
  [Serializable]
  public enum Direction
  {
    Bottom = 1 << 0, // default
    Left = 1 << 1,
    Right = 1 << 2,
    Top = 1 << 3,
    None = 1 << 4
  }

  public static class DirectionExtension
  {
    public static bool IsNone(this Direction direction)
    {
      return direction == Direction.None;
    }

    public static bool IsValidDirection(this Direction direction)
    {
      return direction.IsLeft() || direction.IsRight() || direction.IsTop() || direction.IsBottom();
    }

    public static bool IsHorizontal(this Direction direction)
    {
      return IsLeft(direction) || IsRight(direction);
    }

    public static bool IsVertical(this Direction direction)
    {
      return IsTop(direction) || IsBottom(direction);
    }

    public static bool IsLeft(this Direction direction)
    {
      return direction == Direction.Left;
    }

    public static bool IsRight(this Direction direction)
    {
      return direction == Direction.Right;
    }

    public static bool IsTop(this Direction direction)
    {
      return direction == Direction.Top;
    }

    public static bool IsBottom(this Direction direction)
    {
      return direction == Direction.Bottom;
    }

    public static Direction Invert(this Direction direction)
    {
      switch (direction)
      {
        case Direction.Bottom: return Direction.Top;
        case Direction.Top: return Direction.Bottom;
        case Direction.Left: return Direction.Right;
        case Direction.Right: return Direction.Left;
      }
      return direction;
    }

    public static Direction Left(this Direction direction)
    {
      switch (direction)
      {
        case Direction.Top:
        case Direction.Bottom:
          return Direction.Left;
        case Direction.Left:
        case Direction.Right:
          return Direction.Bottom;
      }
      return direction;
    }

    public static Direction Right(this Direction direction)
    {
      switch (direction)
      {
        case Direction.Top:
        case Direction.Bottom:
          return Direction.Right;
        case Direction.Left:
        case Direction.Right:
          return Direction.Top;
      }
      return direction;
    }
  }
}
