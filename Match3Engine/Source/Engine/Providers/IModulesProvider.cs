using Match3.Engine.Modules;

namespace Match3.Engine.Providers
{
  public interface IModulesProvider
  {
    EngineModule[] PreTick { get; }
    EngineModule[] PostTick { get; }
    EngineModule[] FinalizeTick { get; }
  }
}
