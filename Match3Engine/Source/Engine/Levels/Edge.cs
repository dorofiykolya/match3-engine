using System;
using System.Reflection;
using Match3.Engine.Descriptions.Levels;

namespace Match3.Engine.Levels
{
  public class Edge
  {
    private readonly ITileGridProvider _grid;

    public Edge(ITileGridProvider grid, PositionConverter converter, LevelEdgeDescription description, int objectId)
    {
      _grid = grid;

      Index = description.Index;
      ObjectId = objectId;
      Type = description.Type;
      Position = new Position(description.Position);
      Direction = description.Direction;
      Orientation = converter.GetEdgeOrientationByPosition(description.Position);

      if (Direction != Direction.None &&
        (Direction.IsHorizontal() && Orientation == Orientation.Vertical ||
        Direction.IsVertical() && Orientation == Orientation.Horizontal))
        throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": направление границы не соответствует ее положению(ориентации)");

      if (Type == EdgeType.Input)
      {
        if (Direction == Direction.None)
        {
          throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": граница, которая генерирует новые ячейки не может не иметь направления генерирования!!!");
        }
        if (grid.GetTileByEdge(Position, Direction) == null)
        {
          throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": граница, которая генерирует новые ячейки не может иметь напраление в пустоту!!!");
        }
      }
    }

    public Edge(ITileGridProvider grid, PositionConverter converter, Point edgePosition)
    {
      _grid = grid;
      Type = EdgeType.None;
      Position = new Position(edgePosition);
      Orientation = converter.GetEdgeOrientationByPosition(edgePosition);
      Direction = Direction.None;
    }

    public byte Index { get; private set; }
    public int ObjectId { get; private set; }
    public Direction Direction { get; private set; }
    public Orientation Orientation { get; private set; }
    public Position Position { get; private set; }
    public EdgeType Type { get; private set; }

    public Tile Left { get { return _grid.GetTileByEdge(Position, Direction.Left); } }
    public Tile Right { get { return _grid.GetTileByEdge(Position, Direction.Right); } }
    public Tile Top { get { return _grid.GetTileByEdge(Position, Direction.Top); } }
    public Tile Bottom { get { return _grid.GetTileByEdge(Position, Direction.Bottom); } }
    public bool IsTeleport { get { return Type == EdgeType.TeleportInput || Type == EdgeType.TeleportOutput; } }

    public Tile GetTile(Direction direction)
    {
      return _grid.GetTileByEdge(Position, direction);
    }

    public static int GetCount(int tiles)
    {
      return tiles + 1;
    }
  }
}
