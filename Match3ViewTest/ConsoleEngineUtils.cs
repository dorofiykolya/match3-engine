using System;
using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3ViewTest
{
  public class ConsoleEngineUtils
  {
    private static readonly Dictionary<long, ConsoleColor> _itemsColor = new Dictionary<long, ConsoleColor>
    {
      { GetColorKey(0, 0), ConsoleColor.Red },
      { GetColorKey(0, 1), ConsoleColor.DarkRed },
      { GetColorKey(1, 0), ConsoleColor.Blue },
      { GetColorKey(1, 1), ConsoleColor.DarkBlue },
      { GetColorKey(2, 0), ConsoleColor.Green },
      { GetColorKey(2, 1), ConsoleColor.DarkGreen },
      { GetColorKey(3, 0), ConsoleColor.Yellow },
      { GetColorKey(3, 1), ConsoleColor.DarkYellow },
      { GetColorKey(4, 0), ConsoleColor.Cyan },
      { GetColorKey(4, 1), ConsoleColor.DarkCyan },
      { GetColorKey(5, 0), ConsoleColor.Magenta },
      { GetColorKey(5, 1), ConsoleColor.DarkMagenta }
    };

    private static long GetColorKey(int itemId, int itemLevel)
    {
      return ((long)itemId << 32) | (long)itemLevel;
    }

    public static ConsoleColor GetColor(int itemId, int itemLevel)
    {
      return _itemsColor[GetColorKey(itemId, itemLevel)];
    }

    public static char GetEdge(EdgeType edgeType, Direction direction)
    {
      return ' ';
    }

    public static ConsoleColor GetColor(EdgeType edgeType)
    {
      switch (edgeType)
      {
        case EdgeType.Input: return ConsoleColor.Magenta;
        case EdgeType.Lock: return ConsoleColor.Red;
        case EdgeType.None: return ConsoleColor.Black;
        case EdgeType.Output: return ConsoleColor.Green;
        case EdgeType.TeleportInput: return ConsoleColor.Yellow;
        case EdgeType.TeleportOutput: return ConsoleColor.DarkYellow;
      }
      return ConsoleColor.White;
    }
  }
}
