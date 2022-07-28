using System;
using System.Reflection;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class MatchModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid(TickInvalidation.Fall, TickInvalidation.Generation))
      {
        var grid = state.TileGrid;

        if (state.Environment.IsDebug())
        {
          foreach (var tile in grid.Tiles)
          {
            if (tile.IsEmpty && tile.IsMovable)
            {
              if (FindGenerateEdge(tile, grid))
              {
                throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name +
                                                    ": не правильно отработали модули перемещения ячеек, есть ячейки пустые, если по логике игры такая возможность доступна - нужно добавить поиск, что эти ячейки никогда не смогут быть заполнены иначе это ошибка.");
              }
            }
          }
        }

        var matches = state.Pool.GetMatchCombinationsResult();
        var matchProvider = engine.Configuration.Providers.MatchesProvider;
        foreach (var tile in grid.Tiles)
        {
          if (!tile.IsEmpty)
          {
            matchProvider.Match(tile.Position, grid, matches);
          }
        }
        if (matches.HasMatches)
        {
          state.Invalidate();

          ActivationResult activateResult = null;
          if (engine.Environment.IsGenerateOutputEvents())
          {
            activateResult = new ActivationResult();
          }

          matches.Merge(state.Pool);
          state.TileGridActivator.Activate(matches.Combinations, activateResult);

          state.Streak.AddMatches(matches.Combinations);

          if (engine.Environment.IsGenerateOutputEvents() && activateResult != null)
          {
            var evt = engine.EnqueueByFactory<ActivateEvent>(currentTick);
            evt.InitializeFrom(activateResult.Queue);
          }

          tickState.Invalidate(TickInvalidation.MatchItems);
        }
      }
    }

    private bool FindGenerateEdge(Tile tile, TileGrid grid)
    {
      return false;
    }
  }
}
