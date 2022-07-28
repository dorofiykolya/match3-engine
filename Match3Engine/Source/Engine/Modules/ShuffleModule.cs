using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class ShuffleModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid(TickInvalidation.Fall, TickInvalidation.Generation, TickInvalidation.MatchItems, TickInvalidation.Collect))
      {
        if (!state.AvailableSwaps())
        {
          ShuffleResult shuffleResult = null;
          if (engine.Environment.IsGenerateOutputEvents())
          {
            shuffleResult = new ShuffleResult();
          }

          if (state.TileGrid.Shuffle(shuffleResult))
          {
            state.Invalidate();
          }
          if (!state.IsInvalid)
          {
            var matchProvider = engine.Configuration.Providers.MatchesProvider;
            foreach (var tile in state.TileGrid.Tiles)
            {
              if (!tile.IsEmpty)
              {
                if (matchProvider.Match(tile.Position, state.TileGrid))
                {
                  state.Invalidate();
                  break;
                }
              }
            }
          }

          if (engine.Environment.IsGenerateOutputEvents() && shuffleResult != null)
          {
            var evt = engine.EnqueueByFactory<ShuffleEvent>(currentTick);
            evt.InitializeFrom(shuffleResult.Shuffles);
          }

          tickState.Invalidate(TickInvalidation.Shuffle);
        }
      }
    }
  }
}