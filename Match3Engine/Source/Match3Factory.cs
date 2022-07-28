using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Engine.Providers;

namespace Match3
{
  public class Match3Factory
  {
    private readonly IEngineProviders _providers;
    private readonly EngineEnvironment _environment;

    public Match3Factory(IEngineProviders providers, EngineEnvironment environment)
    {
      _providers = providers;
      _environment = environment;
    }

    public IEngine CreateInstance(LevelDescription level, int energy, Spell[] spells, int maxTicks = 1000)
    {
      var configuration = new Configuration(_environment)
      {
        LevelDescription = level,
        Energy = energy,
        MaxTicks = maxTicks,
        Spells = spells,
        Providers = _providers
      };
      var engine = new Engine.Engine(configuration);
      return engine;
    }
  }
}
