using Match3.Engine.Levels;

namespace Match3.Editor.Utils
{
  public class CoordinateConverter
  {
    public static readonly double TileSize = 50.0;
    public static readonly double EdgeSize = 10.0;
    public static readonly double EdgeOffset = 10.0;
    public static readonly double ContainerWidth = TileSize * 10;
    public static readonly double ContainerHeight = TileSize * 10;

    public static readonly double EditorTileSize = 70.0;
    public static readonly double EditorEdgeSize = 20.0;
    public static readonly double EditorContainerWidth = EditorTileSize * 10;
    public static readonly double EditorContainerHeight = EditorTileSize * 10;
    
    public System.Windows.Point ToCanvasEdge(Point edgePosition, bool editor = false)
    {
      var result = new System.Windows.Point();
      var edgeSize = editor ? EditorEdgeSize : EdgeSize;
      var tileSize = editor ? EditorTileSize : TileSize;
      result.X = ((edgePosition.X / 2) | 0) * tileSize + (edgePosition.X % 2 != 0 ? edgeSize / 2 : -edgeSize / 2);
      result.Y = ((edgePosition.Y / 2) | 0) * tileSize + (edgePosition.Y % 2 != 0 ? edgeSize / 2 : -edgeSize / 2);
      return result;
    }

    public Point ToEngineEdge(System.Windows.Point position)
    {
      var result = new Point();
      result.X = (int)(position.X / TileSize);
      result.Y = (int)(position.Y / TileSize);
      return result;
    }

    public System.Windows.Point ToCanvas(Point position, bool editor = false)
    {
      var result = new System.Windows.Point();
      var tileSize = editor ? EditorTileSize : TileSize;
      result.X = position.X * tileSize;
      result.Y = position.Y * tileSize;
      return result;
    }

    public Point ToEngine(System.Windows.Point position)
    {
      var result = new Point();
      result.X = (int)(position.X / TileSize);
      result.Y = (int)(position.Y / TileSize);
      return result;
    }
  }
}
