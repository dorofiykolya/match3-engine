using Match3.Engine.Levels;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.Utils
{
  public class DirectionToPackIcon
  {
    public static PackIconKind ToPackIcon(Direction direction)
    {
      switch (direction)
      {
        case Direction.Bottom: return PackIconKind.ArrowDown;
        case Direction.Top: return PackIconKind.ArrowUp;
        case Direction.Left: return PackIconKind.ArrowLeft;
        case Direction.Right: return PackIconKind.ArrowRight;
      }
      return PackIconKind.ArrowAll;
    }
  }
}
