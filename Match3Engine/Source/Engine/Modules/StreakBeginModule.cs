namespace Match3.Engine.Modules
{
  public class StreakBeginModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (tickState.IsValid())
      {
        state.Streak.Begin();
      }
    }
  }
}
