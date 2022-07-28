namespace Match3.Editor.Utils.Coroutine
{
  public interface ITimeProvider
  {
    double DeltaTime { get; }
    double TimeScale { get; set; }
  }
}
