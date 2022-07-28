namespace Match3.Editor.Utils.Coroutine
{
  public class WaitYieldCoroutine : YieldCoroutine
  {
    private double _totalTime;
    private readonly ITimeProvider _timeProvider;

    public WaitYieldCoroutine(double seconds, ITimeProvider timeProvider)
    {
      _totalTime = seconds;
      _timeProvider = timeProvider;
    }

    public override bool NeedToSkip
    {
      get
      {
        _totalTime -= _timeProvider.DeltaTime;
        return _totalTime > 0;
      }
    }
  }
}
