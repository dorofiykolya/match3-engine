using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;

namespace Match3.Engine.Utils
{
  public class BoundsUtils
  {
    public static Bounds Calculate(LevelTileDescription[] tiles)
    {
      var bounds = new Bounds
      {
        MinX = int.MaxValue,
        MinY = int.MaxValue,
        MaxX = int.MinValue,
        MaxY = int.MinValue
      };

      foreach (var tile in tiles)
      {
        var position = tile.Position;
        if (position.X < bounds.MinX) bounds.MinX = position.X;
        if (position.X > bounds.MaxX) bounds.MaxX = position.X;

        if (position.Y < bounds.MinY) bounds.MinY = position.Y;
        if (position.Y > bounds.MaxY) bounds.MaxY = position.Y;
      }

      return bounds;
    }
  }
}
