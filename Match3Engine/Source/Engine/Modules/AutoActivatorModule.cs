using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class AutoActivatorModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (state.IsAutoActivate)
      {
        var grid = state.TileGrid;
        var activator = state.TileGridActivator;

        ActivationResult result = null;
        if (engine.Environment.IsGenerateOutputEvents())
        {
          result = new ActivationResult();
        }

        foreach (var tile in grid.Tiles)
        {
          if (!tile.IsEmpty)
          {
            if (tile.ItemType == ItemType.UniversalSwapCell)
            {
              activator.Activate(tile.Position, result);
              state.Invalidate();
            }
            else if (tile.ItemType == ItemType.Cell && tile.Item.Level > LevelId.L0)
            {
              activator.Activate(tile.Position, result);
              state.Invalidate();
            }
          }
        }

        state.FillSwaps();

        state.Invalidate();

        if (engine.Environment.IsGenerateOutputEvents())
        {
          var evt = engine.EnqueueByFactory<SwapsChangedEvent>(currentTick);
          evt.Used = state.Swaps;
          evt.Total = state.MaxSwaps;
        }

        if (engine.Environment.IsGenerateOutputEvents() && result != null)
        {
          var evt = engine.EnqueueByFactory<ActivateEvent>(currentTick);
          evt.InitializeFrom(result.Queue);
        }
      }
    }
  }
}
