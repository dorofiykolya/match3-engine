using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Match3.Engine.Levels;

namespace Match3.Engine.Utils
{
  public class PatternParser
  {
    public enum Cell
    {
      Empty,
      Fill,
      Pivot
    }

    public static Point[] Parse(string pattern, bool byPivot = false)
    {
      var cells = ParseToCell(pattern);
      return ConvertToOffset(cells, byPivot);
    }

    private static Point[] ConvertToOffset(Cell[,] pattern, bool byPivot = false)
    {
      Point? findPivot = null;
      var list = new List<Point>();
      for (var x = 0; x < pattern.GetLength(0); x++)
      {
        for (var y = 0; y < pattern.GetLength(1); y++)
        {
          var cell = pattern[x, y];
          if (cell != Cell.Empty)
          {
            if (byPivot && cell == Cell.Pivot) findPivot = new Point(x, y);
            list.Add(new Point(x, y));
          }
        }
      }
      if (byPivot)
      {
        if (!findPivot.HasValue) throw new ArgumentException("pivot \"X\" не найден");

        var result = new Point[list.Count];
        var pivot = (Point)findPivot;
        for (int i = 0; i < list.Count; i++)
        {
          result[i] = list[i] - pivot;
        }
        return result;
      }
      return list.ToArray();
    }

    public static Cell[,] ParseToCell(string pattern)
    {
      var result = new List<List<Cell>>();
      var reader = new PatternReader(pattern);
      var x = 0;
      var y = 0;
      var opened = false;
      PatternToken prevToken = null;
      while (reader.Next())
      {
        if (!reader.Token.Skip)
        {
          if (prevToken != null)
          {
            if (!prevToken.ExpectedNextToken.Contains(reader.Token.Token))
            {
              throw new ArgumentException(string.Format("not the correct format, expected:\"{0}\", but result is:\"{1}\"", new string(prevToken.ExpectedNextToken), reader.Token.Token));
            }
          }
          
          switch (reader.Token.Token)
          {
            case PatternToken.BEGIN:
              if (opened) throw new ArgumentException("token opened already");
              x = 0;
              opened = true;
              result.Add(new List<Cell>());
              break;
            case PatternToken.END:
              if (!opened) throw new ArgumentException("token not opened END");
              opened = false;
              y++;
              break;
            case PatternToken.DELIM:
              if (!opened) throw new InvalidOperationException("token not opened DELIM");
              //result[y].Add(PatternToken.EMPTY);
              break;
            case PatternToken.EMPTY:
              if (opened)
              {
                if(prevToken != null && prevToken.Token == PatternToken.EMPTY) throw new ArgumentException(string.Format("not the correct format, expected:\"{0}\", but result is:\"{1}\"", new string(prevToken.ExpectedNextToken), reader.Token.Token));
                result[y].Add(reader.Token.ToValue());
              }
              break;
            case PatternToken.EXIST:
            case PatternToken.PIVOT:
              if (!opened) throw new InvalidOperationException("token not opened VALUE");
              result[y].Add(reader.Token.ToValue());
              break;
          }

          prevToken = reader.Token;
        }
      }
      if (opened) throw new InvalidOperationException();
      if (!result.TrueForAll(l => l.Count == result[0].Count))
      {
        throw new ArgumentException("result must bee squared");
      }
      if (result.Count == 0) return new Cell[0, 0];

      var output = new Cell[result.Count, result[0].Count];
      x = 0;
      y = 0;
      foreach (var list in result)
      {
        x = 0;
        foreach (var item in list)
        {
          output[y, x] = item;
          x++;
        }
        y++;
      }
      return output;
    }
    private class PatternReader
    {
      private readonly string _pattern;
      private int _index;
      private PatternToken _token;

      public PatternReader(string pattern)
      {
        var pivotCount = 0;
        foreach (var p in pattern)
        {
          switch (p)
          {
            case PatternToken.BEGIN:
            case PatternToken.END:
            case PatternToken.DELIM:
            case PatternToken.EMPTY:
            case PatternToken.EXIST:
              break;
            case PatternToken.PIVOT:
              if (pivotCount >= 1) throw new ArgumentException("pivot \"X\" может быть только 1");
              ++pivotCount;
              break;
            case '\r':
            case '\n':
              //skip
              break;
            default:
              throw new ArgumentException("символ не поддерживается: " + p);

          }
        }

        _pattern = pattern;
      }

      public bool Next()
      {
        var result = _pattern.Length > _index;
        if (result)
        {
          _token = new PatternToken(_pattern[_index], _index);
        }
        _index++;
        return result;
      }

      public PatternToken Token
      {
        get { return _token; }
      }
    }

    private class PatternToken
    {
      public const char BEGIN = '[';
      public const char END = ']';
      public const char DELIM = '|';
      public const char EMPTY = ' ';
      public const char EXIST = '#';
      public const char PIVOT = 'X';

      private static readonly char[] Available = { BEGIN, END, DELIM, EMPTY, EXIST, PIVOT };

      private char _char;
      private readonly int _index;

      public PatternToken(char v, int index)
      {
        _char = v;
        _index = index;
      }

      public int Index { get { return _index; } }

      public bool Skip
      {
        get { return Array.IndexOf(Available, _char) == -1; }
      }

      public char Token
      {
        get
        {
          if (Skip) throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": невозможно использовать эту операцию");
          return _char;
        }
      }

      public Cell ToValue()
      {
        switch (_char)
        {
          case EMPTY: return Cell.Empty;
          case EXIST: return Cell.Fill;
          case PIVOT: return Cell.Pivot;
        }

        throw new InvalidOperationException(MethodBase.GetCurrentMethod().Name + ": формат не поддерживается:" + _char);
      }

      public char[] ExpectedNextToken
      {
        get
        {
          switch (_char)
          {
            case BEGIN:
            case DELIM:
              return new[] { EMPTY, EXIST, PIVOT };
            case END:
              return new[] { BEGIN, EMPTY };
            case EMPTY:
              return new[] { BEGIN, END, DELIM, EMPTY };
            case EXIST:
            case PIVOT:
              return new[] { END, DELIM };
          }
          return null;
        }
      }
    }
  }
}
