using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;

namespace Match3.Analyzer
{
  public class Match3Analyzer
  {
    private readonly Match3Factory _factory;

    public Match3Analyzer(Match3Factory factory)
    {
      _factory = factory;
    }

    public Match3LevelStatistic Analyze(LevelDescription level, AnalyzerCancellation cancellation, IProgressHandler progressHandler = null)
    {
      var result = new Match3LevelStatistic(cancellation);

      var player = new Match3AnalyzerPlayer(() => _factory.CreateInstance(level, 0, new Spell[0]));
      var root = player.FastForward(cancellation, progressHandler);
      var chains = root.GetLastChains();
      result.Total = chains.Count;
      foreach (var chain in chains)
      {
        if (result.Canceled) break;

        var chainState = chain.Engine.State;
        if (chainState.Requirements.IsComplete)
        {
          result.Success++;
          result.Swaps += chainState.Swaps;
          result.SuccessAvarangeScore += chainState.Score.Score;
        }
        else
        {
          result.Fail++;
          result.FailAvarangeScore += chainState.Score.Score;
        }
      }

      if (result.Success != 0)
      {
        result.Swaps /= result.Success;
        result.SuccessAvarangeScore /= result.Success;
      }

      return result;
    }
  }
}
