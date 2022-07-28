namespace Match3.Analyzer
{
  public class Match3LevelStatistic
  {
    private readonly AnalyzerCancellation _cancellation;

    public int Total;
    public int Success;
    public int SuccessAvarangeScore;
    public int FailAvarangeScore;
    public int Fail;
    public int Swaps;

    public Match3LevelStatistic(AnalyzerCancellation analyzerCancellation)
    {
      _cancellation = analyzerCancellation;
    }

    public bool Canceled
    {
      get { return _cancellation.IsCanceled; }
    }

    public float SuccessPercent
    {
      get { return (Success / (float)Total) * 100f; }
    }
  }
}
