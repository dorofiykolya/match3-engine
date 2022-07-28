using Match3.Engine.Levels;

namespace Match3.Engine.Utils
{
  public class MatchPatternParser
  {
    /// <summary>
    /// преобразовует строковый паттерн в оффсеты точки
    /// </summary>
    /// <param name="pattern">
    /// пример паттерна 3х3
    /// "#" - точка является оффсетом
    /// " " - пусто
    /// [#|#|#]
    /// [ |#| ]
    /// [ |#| ]
    /// </param>
    /// <returns></returns>
    public static Point[][] Parse(string pattern)
    {
      var offsetMap = PatternParser.Parse(pattern, false);
      return ToOffsets(offsetMap);
    }

    private static Point[][] ToOffsets(Point[] offsets)
    {
      var result = new Point[offsets.Length][];
      for (var i = 0; i < offsets.Length; i++)
      {
        var pivot = offsets[i];
        var list = new Point[offsets.Length];
        var index = 0;

        foreach (var point in offsets)
        {
          list[index] = point - pivot;
          index++;
        }
        result[i] = list;
      }
      return result;
    }
  }
}
