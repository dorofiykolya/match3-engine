using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class RequirementModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid())
      {
        if (state.Requirement.IsComplete)
        {
          IncreaseLevelEvent evt = null;

          var maxSwaps = state.MaxSwaps;
          var swaps = state.Swaps;

          if (state.Environment.IsGenerateOutputEvents())
          {
            engine.EnqueueByFactory<RequirementsCompleteEvent>(state.Tick).AvailableSwaps = swaps;
          }

          if (swaps < maxSwaps)
          {
            state.StopGenerator();
            state.AutoActivate();

            var tiles = state.TileGrid.Tiles.OrderBy(a => state.GetNextRandom(state.TileGrid.TileCount));
            foreach (var tile in tiles)
            {
              if (swaps >= maxSwaps) break;

              if (!tile.IsEmpty && tile.Item.Level < LevelId.L1 && tile.ItemType == ItemType.Cell)
              {
                tile.SetNextItem(tile.Item.CopyAndIncreaseLevel());
                tile.ApplyNextItem();

                if (engine.Environment.IsGenerateOutputEvents())
                {
                  if (evt == null)
                  {
                    evt = engine.EnqueueByFactory<IncreaseLevelEvent>(currentTick);
                  }
                  evt.Items.Add(new IncreaseLevelEvent.IncreaseItem
                  {
                    Item = tile.Item,
                    Position = tile.Position
                  });
                }
              }
              ++swaps;
            }
            if (swaps < maxSwaps)
            {
              foreach (var tile in tiles)
              {
                if (swaps >= maxSwaps) break;

                if (!tile.IsEmpty && tile.Item.Level == LevelId.L1 && tile.ItemType == ItemType.Cell)
                {
                  tile.SetNextItem(new Item(ItemId.Universal, 0));
                  tile.ApplyNextItem();

                  if (engine.Environment.IsGenerateOutputEvents())
                  {
                    if (evt == null)
                    {
                      evt = engine.EnqueueByFactory<IncreaseLevelEvent>(currentTick);
                    }
                    evt.Items.Add(new IncreaseLevelEvent.IncreaseItem
                    {
                      Item = tile.Item,
                      Position = tile.Position
                    });
                  }
                }
                ++swaps;
              }
            }
            state.Invalidate();
          }
          else
          {
            state.Finish(EngineFinishReason.RequirementComplete);
          }
        }
      }
    }
  }
}
