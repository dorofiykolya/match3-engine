using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  public class EngineScore : IEngineScore
  {
    private readonly EngineState _engineState;
    private readonly TileGrid _tileGrid;
    private readonly IItemDescriptionProvider _itemsProvider;
    private readonly EngineEnvironment _environment;
    private int _score;

    public EngineScore(EngineState engineState, TileGrid tileGrid, IEngineProviders providers, EngineEnvironment environment)
    {
      _engineState = engineState;
      _tileGrid = tileGrid;
      _itemsProvider = providers.ItemsProvider;
      _environment = environment;
      _score = 0;
    }

    public int Score
    {
      get { return _score; }
    }

    public void Add(int value)
    {
      _score += value;
      _engineState.Requirement.Dispatch(_score, LevelRequirementType.Stars);

      if (_environment.IsGenerateOutputEvents())
      {
        _engineState.Output.EnqueueByFactory<ScoreEvent>(_engineState.Tick).Score = _score;
      }
    }

    public void Add(Item item)
    {
      Add(_itemsProvider.Get(item.Id).GetLevel(item.Level).Score);
    }
  }
}
