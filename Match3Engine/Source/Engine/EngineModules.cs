using System.Linq;
using Match3.Engine.Modules;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  public class EngineModules
  {
    private readonly EngineModule[] _preTick;
    private readonly EngineModule[] _postTick;
    private readonly EngineModule[] _finalizeTick;
    private readonly ModuleTickState _tickState;

    public EngineModules(IModulesProvider modules)
    {
      _preTick = modules.PreTick.ToArray();
      _postTick = modules.PostTick.ToArray();
      _finalizeTick = modules.FinalizeTick.ToArray();
      _tickState = new ModuleTickState();
    }

    public void PreTick(Engine engine, int currentTick, EngineState state, int tickStep)
    {
      _tickState.Validate();
      foreach (var module in _preTick)
      {
        module.Tick(engine, currentTick, state, _tickState, tickStep);
      }
    }

    public void PostTick(Engine engine, int currentTick, EngineState state, int tickStep)
    {
      _tickState.Validate();
      foreach (var module in _postTick)
      {
        module.Tick(engine, currentTick, state, _tickState, tickStep);
      }
    }

    public void FinalizeTick(Engine engine, int currentTick, EngineState state, int tickStep)
    {
      _tickState.Validate();
      foreach (var module in _finalizeTick)
      {
        module.Tick(engine, currentTick, state, _tickState, tickStep);
      }
    }
  }
}
