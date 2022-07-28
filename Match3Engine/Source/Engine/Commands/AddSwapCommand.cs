using System;
using Match3.Engine.InputActions;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Commands
{
  public class AddSwapCommand : EngineCommand<AddSwapsInputAction>
  {
    protected override void Execute(AddSwapsInputAction action, Engine engine, IEngineStateInvalidator stateInvalidator)
    {
      if (action.Swaps <= 0)
      {
        throw new InvalidOperationException("Swaps не может быть 0 или отрицательным");
      }
      stateInvalidator.AddAdditionalSwaps(action.Swaps);
      stateInvalidator.Invalidate();

      if (engine.Environment.IsGenerateOutputEvents())
      {
        var evt = engine.EnqueueByFactory<SwapsChangedEvent>(engine.Tick);
        evt.Used = engine.State.Swaps;
        evt.Total = engine.State.MaxSwaps;
      }
    }
  }
}
