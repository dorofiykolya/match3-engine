using System;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class AutoFinishModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid() || state.IsAutoActivate)
      {
        if ((engine.Configuration.Environment & EngineEnvironment.AnalysisOfRequiredSwaps) != EngineEnvironment.AnalysisOfRequiredSwaps)
        {
          if (state.Swaps >= state.MaxSwaps)
          {
            if (state.Requirement.IsComplete)
            {
              if (!state.IsAutoActivate)
              {
                if (state.Environment.IsGenerateOutputEvents())
                {
                  engine.EnqueueByFactory<RequirementsCompleteEvent>(state.Tick).AvailableSwaps = 0;
                }
              }
              state.Finish(EngineFinishReason.RequirementComplete);
            }
            else
            {
              state.Finish(EngineFinishReason.SwapsEnded);
            }
          }
        }
      }
    }
  }
}
