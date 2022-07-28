using Match3.Engine.Descriptions.Levels;

namespace Match3.Engine.Providers
{
  public interface ILevelDescriptionProvider
  {
    int Count { get; }
    LevelDescription Get(int level);
  }
}
