namespace Match3.Analyzer
{
  public class AnalyzerCancellation
  {
    public bool IsCanceled { get; private set; }

    public void Cancel()
    {
      IsCanceled = true;
    }
  }
}
