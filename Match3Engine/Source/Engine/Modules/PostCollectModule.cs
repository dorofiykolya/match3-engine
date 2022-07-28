namespace Match3.Engine.Modules
{
  public class PostCollectModule : CollectModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      Process(engine, currentTick, state, tickState, tickStep);
    }
  }
}
