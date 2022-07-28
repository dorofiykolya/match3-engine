namespace Match3.Engine
{
  public interface IEngineNextRandom
  {
    /// <summary>
    /// рандомное значение
    /// </summary>
    /// <returns></returns>
    bool GetNextRandomBool();

    /// <summary>
    /// рандомное значение
    /// </summary>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    int GetNextRandom(int maxValue);

    /// <summary>
    /// рандомное значение
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    int GetNextRandom(int minValue, int maxValue);
  }
}
