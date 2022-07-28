using Match3.Engine.Modules;
using Match3.Engine.Providers;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedModulesProvider : IModulesProvider
  {
    private readonly EngineModule[] _preTick;
    private readonly EngineModule[] _postTick;
    private readonly EngineModule[] _finalizeTick;

    public SharedModulesProvider()
    {
      _preTick = new EngineModule[]
        {
          new FallModule(),
          new GeneratorModule(),
          new PostFallModule(),
          new ActivatorBeginModule(),
          new MatchModule(),
          new AutoActivatorModule(),
          new CollectModule(),
          new ActivatorEndModule(),
          new ShuffleModule(),
          //new AutoSwapModule(),
          new AutoFinishModule(),
          new RequirementModule(),
          new ActivatorBeginModule(),
        };

      _postTick = new EngineModule[]
      {
        new StreakBeginModule(),
        new PostCollectModule(),
        new ActivatorEndModule(),

        new ActivatorBeginModule(),
        new MatchModule(),
        new CollectModule(),
        new ActivatorEndModule(),
      };

      _finalizeTick = new EngineModule[]
        {
          new FallModule(),
          new GeneratorModule(),
          new PostFallModule(),
          new ActivatorBeginModule(),
          new MatchModule(),
          new AutoActivatorModule(),
          new CollectModule(),
          new ActivatorEndModule(),
          new ShuffleModule(),
          new AutoFinishModule(),
          new StreakEndModule(),
          new RequirementModule(),
        };
    }

    public virtual EngineModule[] PreTick
    {
      get { return _preTick; }
    }

    public virtual EngineModule[] PostTick
    {
      get { return _postTick; }
    }

    public virtual EngineModule[] FinalizeTick
    {
      get { return _finalizeTick; }
    }
  }
}
