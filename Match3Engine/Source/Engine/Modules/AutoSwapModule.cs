using System.Collections.Generic;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;

namespace Match3.Engine.Modules
{
  public class AutoSwapModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid() && state.Swaps < state.MaxSwaps && engine.Actions.Count == 0)
      {
        var list = new List<Swap>();
        if (state.AvailableSwaps(list))
        {
          engine.AddAction(new SwapInputAction
          {
            Swap = list[0],
            Tick = currentTick + 1
          });
        }
      }
    }
  }
}
