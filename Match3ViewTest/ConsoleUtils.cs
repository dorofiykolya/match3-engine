using System;
using System.Collections.Generic;

namespace Match3ViewTest
{
  public class ConsoleUtils
  {
    private static readonly Stack<ConsoleColor> _colorStack = new Stack<ConsoleColor>();

    public static void PushColor()
    {
      _colorStack.Push(Console.ForegroundColor);
    }

    public static void PopColor()
    {
      if (_colorStack.Count != 0)
      {
        Console.ForegroundColor = _colorStack.Pop();
      }
    }
  }
}
