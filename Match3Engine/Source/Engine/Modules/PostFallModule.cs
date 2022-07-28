using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class PostFallModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid(TickInvalidation.Fall, TickInvalidation.Generation))
      {
        FallEvent evt = null;
        var grid = state.TileGrid;
        foreach (var tile in grid.Tiles)
        {
          //if (!tile.IsProcessed)
          {
            //tile.BeginProcess();
            if (!tile.IsEmpty && tile.IsCanReceiveItem && tile.IsMovable)
            {
              Tile next = NextTile(tile, state);
              if (next != null && next.IsCanReceiveItem)
              {
                var nextFall = tile.Next;
                var nextTile = nextFall.Tile;
                if (nextTile == null || NextTile(nextTile, state) == null)
                {
                  //next.BeginProcess();

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
                      MoveTo = tile.Direction,
                      To = next.Position,
                      MoveFrom = next.Direction.Invert(),
                      MoveType = MoveType.Fall,
                      Item = item.Copy()
                    });
                  }

                  state.Invalidate();
                  tickState.Invalidate(TickInvalidation.Fall);

                  break;
                }
              }
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
    }

    private Tile NextTile(Tile tile, EngineState state)
    {
      if (!tile.IsEmpty && tile.IsMovable && tile.IsCanReceiveItem)
      {
        var nextFall = tile.Next;
        var next = nextFall.Tile;
        if (next != null)
        {
          if (next.IsEmpty && next.IsCanReceiveItem && next.IsMovable && next.NextItem == null)
          {
            return next;
          }

          var leftDirection = tile.Direction.Left();
          var rightDirection = tile.Direction.Right();

          var left = state.TileGrid.GetTile(next.Position, leftDirection);
          var right = state.TileGrid.GetTile(next.Position, rightDirection);

          var tileLeft = state.TileGrid.GetTile(tile.Position, leftDirection);
          var tileRight = state.TileGrid.GetTile(tile.Position, rightDirection);

          var leftAvailable = left != null && left.IsEmpty && left.NextItem == null && (tileLeft == null || (tileLeft.IsMovable && tileLeft.IsCanReceiveItem));
          var rightAvailable = right != null && right.IsEmpty && right.NextItem == null && (tileRight == null || (tileRight.IsMovable && tileRight.IsCanReceiveItem));
          if (leftAvailable && rightAvailable)
          {
            return state.NextLeftOrRight().IsLeft() ? left : right;
          }
          if (leftAvailable)
          {
            return left;
          }
          if (rightAvailable)
          {
            return right;
          }
        }
      }
      return null;
    }
  }
}
