using System;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Commands
{
  public class SwapCommand : EngineCommand<SwapInputAction>
  {
    protected override void Execute(SwapInputAction action, Engine engine, IEngineStateInvalidator stateInvalidator)
    {
      if (action.Swap == null) throw new ArgumentNullException("");

      if (!engine.Configuration.Providers.MatchesProvider.Match(action.Swap, stateInvalidator.GridProvider))
      {
        throw new InvalidOperationException("операция невозможна, нельзя перемещать данные ячейки, комбинации не найдены: " + action.Swap);
      }

      stateInvalidator.Invalidate();

      ActivationResult result = null;
      if (engine.Environment.IsGenerateOutputEvents())
      {
        result = new ActivationResult();
      }

      if (engine.Environment.IsGenerateOutputEvents() && result != null)
      {
        var evt = engine.EnqueueByFactory<PreSwapEvent>(action.Tick);
        evt.Swap = action.Swap;
        evt.Total = stateInvalidator.MaxSwaps;
        evt.Used = stateInvalidator.Swaps;
      }

      stateInvalidator.Swap(action.Swap, result);

      if (engine.Environment.IsGenerateOutputEvents() && result != null)
      {
        var evt = engine.EnqueueByFactory<PostSwapEvent>(action.Tick);
        evt.Swap = action.Swap;
        evt.Total = stateInvalidator.MaxSwaps;
        evt.Used = stateInvalidator.Swaps;
      }

      if (engine.Environment.IsGenerateOutputEvents() && result != null)
      {
        engine.EnqueueByFactory<ActivateEvent>(action.Tick).InitializeFrom(result.Queue);
      }
    }
  }
}
