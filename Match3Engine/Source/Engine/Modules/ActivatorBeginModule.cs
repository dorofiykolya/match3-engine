namespace Match3.Engine.Modules
{
  public class ActivatorBeginModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      state.TileGridActivator.Begin();
    }
  }
}
