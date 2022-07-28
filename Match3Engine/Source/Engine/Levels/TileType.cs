namespace Match3.Engine.Levels
{
  public enum TileType
  {
    /// <summary>
    /// ячейка в которой эелементы можно передвигать, а так же сами элементы могут передвигаться
    /// </summary>
    Movable = 0, // default
    
    /// <summary>
    /// с ячейкой нет никаких действий
    /// </summary>
    Static = 1,

    /// <summary>
    /// ячейка заблокирована, в ней элементы нельзя передвигать (свапать) но сами элементы передвигаются (падают)
    /// </summary>
    Locked = 2
  }
}
