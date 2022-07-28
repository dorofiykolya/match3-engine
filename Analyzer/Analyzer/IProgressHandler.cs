namespace Match3.Analyzer
{
  public interface IProgressHandler
  {
    void IncreaseSwaps();
    void IncreaseVariant();
    void IncreaseSuccess();
    void IncreaseFail();
  }
}
