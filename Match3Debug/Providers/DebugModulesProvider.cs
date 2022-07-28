using System.Collections.Generic;
using Match3.Engine.Modules;
using Match3.Engine.Shareds.Providers;
using Match3Debug.Modules;

namespace Match3Debug.Providers
{
  public class DebugModulesProvider : SharedModulesProvider
  {
    private readonly EngineModule[] _preTick;
    private readonly EngineModule[] _postTick;
    private readonly EngineModule[] _finilizeTick;

    public DebugModulesProvider()
    {
      var module = new DebugTileGridOutputModule();

      var list = new List<EngineModule>(base.PreTick);
      list.Add(module);
      _preTick = list.ToArray();

      list = new List<EngineModule>(base.PostTick);
      list.Insert(0, module);
      list.Add(module);
      _postTick = list.ToArray();

      list = new List<EngineModule>(base.PostTick);
      list.Insert(0, module);
      list.Add(module);
      _finilizeTick = list.ToArray();
    }

    public override EngineModule[] PreTick
    {
      get { return _preTick; }
    }

    public override EngineModule[] PostTick
    {
      get { return _postTick; }
    }

    public override EngineModule[] FinalizeTick
    {
      get { return _finilizeTick; }
    }
  }
}
