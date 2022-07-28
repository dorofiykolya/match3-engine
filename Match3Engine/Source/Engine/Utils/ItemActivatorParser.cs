using Match3.Engine.Levels;

namespace Match3.Engine.Utils
{
  public class ItemActivatorParser
  {
    /// <summary>
    /// преобразовует паттерн в оффсеты относительно точки заданой в паттерне
    /// </summary>
    /// <param name="pattern">
    /// "#" - оффсет
    /// "X" - pivot, точка относительно которой строится оффсет
    /// " " - пусто
    /// [#|#|#]
    /// [ |X| ]
    /// [#|#|#]
    /// </param>
    /// <returns></returns>
    public static Point[] Parse(string pattern)
    {
      return PatternParser.Parse(pattern, true);
    }
  }
}
