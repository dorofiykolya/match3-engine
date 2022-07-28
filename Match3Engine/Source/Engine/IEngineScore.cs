using Match3.Engine.Levels;

namespace Match3.Engine
{
  public interface IEngineScore
  {
    int Score { get; }
    void Add(int value);
    void Add(Item item);
  }
}
