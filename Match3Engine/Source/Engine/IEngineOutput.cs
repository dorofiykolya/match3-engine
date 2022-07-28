using Match3.Engine.OutputEvents;

namespace Match3.Engine
{
  public interface IEngineOutput
  {
    int Count { get; }
    OutputEvent Dequeue();
    void ReleaseToPool(OutputEvent evt);
  }
}
