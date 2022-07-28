using System;
using System.Reflection;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class FallModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      FallEvent evt = null;

      var grid = state.TileGrid;

      foreach (var tile in grid.Tiles)
      {
        if (!tile.IsEmpty && tile.IsMovable)
        {
          var nextFall = tile.Next;
          var next = nextFall.Tile;
          if (next != null && !next.IsProcessed && next.IsEmpty && next.IsCanReceiveItem)
          {
            next.BeginProcess();
            tile.BeginProcess();
            var item = tile.Item;
            tile.SetNextItem(null);
            next.SetNextItem(item);

            if (engine.Environment.IsGenerateOutputEvents())
            {
              if (evt == null)
              {
                evt = engine.EnqueueByFactory<FallEvent>(currentTick);
              }
              evt.Items.Add(new FallEvent.FallItem
              {
                From = tile.Position,
                To = next.Position,
                MoveType = nextFall.MoveType,
                MoveTo = nextFall.MoveTo,
                MoveFrom = nextFall.MoveFrom,
                Item = item.Copy()
              });
            }

            state.Invalidate();
            tickState.Invalidate(TickInvalidation.Fall);
          }
        }
      }
      foreach (var tile in grid.Tiles)
      {
        tile.ApplyNextItem();
      }

      foreach (var tile in grid.Tiles)
      {
        tile.EndProcess();
      }
    }

    private Tile FindEmpty(Tile tile)
    {
      while (tile != null)
      {
        if (tile.IsEmpty && tile.IsMovable)
        {
          return tile;
        }
        tile = tile.Prev.Tile;
      }
      return null;
    }

    private Tile FindLastEmpty(Tile tile)
    {
      if (!tile.IsEmpty) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": поиск пустой ячейки с ячейки, которая сама не является пустой");

      var result = tile;
      while (tile != null)
      {
        if (!(tile.IsEmpty && tile.IsMovable))
        {
          break;
        }
        result = tile;
        tile = tile.Prev.Tile;
      }

      return result;
    }
  }
}
