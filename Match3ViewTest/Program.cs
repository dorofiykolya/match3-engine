using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Match3ViewTest;

namespace ViewTest
{
  public class Program
  {
    private static readonly Dictionary<string, Action> _map = new Dictionary<string, Action>
    {
      {"Match3 - test scene 4x4", ()=> ConsoleTestEngine.Test(4,4) },
      {"Match3 - test scene 5x5", ()=> ConsoleTestEngine.Test(5,5) },
      {"Match3 - test scene 6x6", ()=> ConsoleTestEngine.Test(6,6) },
      {"Match3 - test scene 7x7", ()=> ConsoleTestEngine.Test(7,7) },
      {"Match3 - test scene 8x8", ()=> ConsoleTestEngine.Test(8,8) },
      {"Match3 - test scene 9x9", ()=> ConsoleTestEngine.Test(9,9) },
      {"Match3 - test scene 10x10", ()=> ConsoleTestEngine.Test(10,10) },
      {"TileGridGenerator 4x4", () => ConsoleTestTileGridGenerator.Test(4,4) },
      {"TileGridGenerator 5x5", () => ConsoleTestTileGridGenerator.Test(5,5) },
      {"TileGridGenerator 6x7", () => ConsoleTestTileGridGenerator.Test(6,7) },
      {"TileGridGenerator 9x3", () => ConsoleTestTileGridGenerator.Test(9,3) },
      {"TileGridGenerator 4x7", () => ConsoleTestTileGridGenerator.Test(4,7) },
      {"TileGridGenerator 10x10", () => ConsoleTestTileGridGenerator.Test(10,10) },
      {"Exit", ()=> Environment.Exit(0) }
    };

    public static void Main(string[] args)
    {
      Console.OutputEncoding = Encoding.UTF8;
      var currentIndex = 0;
      var keys = _map.Keys.ToArray();
      while (true)
      {
        Console.Clear();
        Console.WriteLine("Press Escape to exit");
        Console.WriteLine("Press Up/Down Arrow to navigate");
        Console.WriteLine("Press Enter to apply");
        Console.WriteLine("");
        for (int i = 0; i < keys.Length; i++)
        {
          ConsoleUtils.PushColor();
          var key = keys[i];
          if (currentIndex == i)
          {
            Console.ForegroundColor = ConsoleColor.Magenta;
          }
          Console.WriteLine($"<{key}>");

          ConsoleUtils.PopColor();
        }
        var keyInfo = Console.ReadKey(false);
        switch (keyInfo.Key)
        {
          case ConsoleKey.Enter:
            _map[keys[currentIndex]]();
            break;
          case ConsoleKey.UpArrow:
            currentIndex--;
            if (currentIndex < 0) currentIndex = 0;
            break;
          case ConsoleKey.DownArrow:
            currentIndex++;
            if (currentIndex >= keys.Length) currentIndex = keys.Length - 1;
            break;
          case ConsoleKey.Escape:
            return;
        }
      }
    }
  }
}
