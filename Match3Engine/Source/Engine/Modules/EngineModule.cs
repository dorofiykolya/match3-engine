namespace Match3.Engine.Modules
{
  public abstract class EngineModule
  {
    public abstract void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep);
  }
}
