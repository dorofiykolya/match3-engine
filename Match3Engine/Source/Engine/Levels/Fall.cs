namespace Match3.Engine.Levels
{
  public struct Fall
  {
    public static readonly Fall Empty = new Fall();

    public Tile Tile;
    public MoveType MoveType;
    public Direction MoveTo;
    public Direction MoveFrom;

    public Fall(Tile tile, MoveType moveType, Direction moveTo, Direction moveFrom)
    {
      Tile = tile;
      MoveType = moveType;
      MoveTo = moveTo;
      MoveFrom = moveFrom;
    }

    public bool IsEmpty
    {
      get { return Tile == null; }
    }
  }
}
