using System.Collections.Generic;
using Match3.Engine;
using Match3.Engine.Levels;
using Match3.Engine.Modules;

namespace Match3Debug.Modules
{
  public class DebugTileGridOutputModule : EngineModule
  {
    public class DebugTile
    {
      public Point Position;
      public Item Item;
      public TileType Type;
    }

    public class DebugEdge
    {
      public Point Position;
      public Orientation Orientation;
      public EdgeType Type;
      public Direction Direction;
    }

    public class DebugMatch
    {

    }

    public class DebugTileGrid
    {
      public Bounds Bounds;
      public int Tick;
      public int MaxSwaps;
      public int Swaps;
      public int Score;
      public List<DebugTile> Tiles = new List<DebugTile>();
      public List<DebugEdge> Edges = new List<DebugEdge>();
      public List<DebugMatch> Matches = new List<DebugMatch>();
    }

    public readonly List<DebugTileGrid> Steps = new List<DebugTileGrid>();
    public int CurrentStep;

    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      var grid = new DebugTileGrid();
      grid.Tick = currentTick;
      grid.MaxSwaps = state.MaxSwaps;
      grid.Swaps = state.Swaps;
      grid.Score = state.Score.Score;
      grid.Bounds = state.TileGrid.Bounds;

      Steps.Add(grid);

      foreach (var tile in state.TileGrid.Tiles)
      {
        grid.Tiles.Add(new DebugTile
        {
          Position = tile.Position,
          Item = tile.Item,
          Type = tile.Type
        });
      }
      foreach (var edge in state.TileGrid.Edges)
      {
        grid.Edges.Add(new DebugEdge
        {
          Type = edge.Type,
          Position = edge.Position,
          Orientation = edge.Orientation,
          Direction = edge.Direction
        });
      }


    }
  }
}
