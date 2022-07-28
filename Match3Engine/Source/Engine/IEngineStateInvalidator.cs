using Match3.Engine.Levels;
namespace Match3.Engine
{
  public interface IEngineStateInvalidator
  {
    /// <summary>
    /// инвалидировать состояние (для повторного прощета стека модулей)
    /// </summary>
    void Invalidate();

    /// <summary>
    /// передвинуть
    /// </summary>
    /// <param name="swap"></param>
    /// <param name="result"></param>
    void Swap(Swap swap, ActivationResult result = null);

    /// <summary>
    /// использовать спел
    /// </summary>
    /// <param name="spell"></param>
    void UseSpell(UseSpell spell);

    /// <summary>
    /// добавить дополнительные шаги
    /// </summary>
    /// <param name="swaps">к-во добавляемых шагов</param>
    void AddAdditionalSwaps(int swaps);

    /// <summary>
    /// добавить дополнительную энергию
    /// </summary>
    /// <param name="energy"></param>
    void AddAdditionalEnergy(int energy);

    /// <summary>
    /// провайер сетки
    /// </summary>
    ITileGridProvider GridProvider { get; }

    /// <summary>
    /// максимальное к-во шагов
    /// </summary>
    int MaxSwaps { get; }

    /// <summary>
    /// к-во использованых шагов
    /// </summary>
    int Swaps { get; }

    /// <summary>
    /// к-во энергии
    /// </summary>
    int Energy { get; }

    /// <summary>
    /// продолжить игру с дополнительным к-вом шагов
    /// </summary>
    /// <param name="additionalSwaps">дополнительное к-во шагов</param>
    void Continue(int additionalSwaps);

    /// <summary>
    /// добавить спелл
    /// </summary>
    /// <param name="id"></param>
    /// <param name="level"></param>
    /// <param name="count"></param>
    void AddSpell(int id, int level, int count);
  }
}
