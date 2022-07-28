using System;
using System.Linq;
using System.Text;
using Match3.Engine.Levels;
using Match3.Engine.Utils;
using Match3Debug;

namespace Match3ViewTest
{
  public class ConsoleTestTileGridGenerator
  {
    private static int _iteration;

    public static void Test(int width, int height)
    {
      Console.OutputEncoding = Encoding.UTF8;
      _iteration = 0;
      Draw(width, height);
      while (true)
      {
        var key = Console.ReadKey(false).Key;
        switch (key)
        {
          case ConsoleKey.Escape:
            return;
          case ConsoleKey.LeftArrow:
            _iteration--;
            if (_iteration < 0) _iteration = 0;
            Draw(width, height);
            break;
          case ConsoleKey.RightArrow:
            _iteration++;
            Draw(width, height);
            break;
        }
      }
    }

    public static void Draw(int width, int height)
    {
      Console.Clear();
      Console.WriteLine($"step: {_iteration} press left/right arrow button to change iteration");

      var factory = new TestConfigurationFactory();
      var converter = new PositionConverter();
      var matches = factory.CreateMatchProvder();
      var providers = factory.CreateProviders(10, width, height);
      var descriptions = factory.CreateItemsProvider();
      var generator = new TileGridGenerator(0, providers);
      var grid = generator.GenerateNext(width, height, descriptions.Collection.Select(i => i.Id).ToArray(), _iteration);

      ConsoleUtils.PushColor();
      Console.ForegroundColor = ConsoleColor.Yellow;
      foreach (var message in grid.WarningMessages)
      {
        Console.WriteLine(message);
      }
      ConsoleUtils.PopColor();

      ConsoleUtils.PushColor();
      var topOffset = 3 + grid.WarningMessages.Count();
      foreach (var tile in grid.Tiles)
      {
        if (tile.Item != null)
        {
          var position = converter.TileToEdgeInternal(tile.Position);
          var color = ConsoleEngineUtils.GetColor(tile.Item.Id, tile.Item.Level);
          Console.ForegroundColor = color;
          Console.SetCursorPosition(position.X, position.Y + topOffset);
          Console.Write("█");
        }
      }
      foreach (var edge in grid.Edges)
      {
        var color = ConsoleEngineUtils.GetColor(edge.Type);
        var symbol = edge.Direction.IsHorizontal() ? "|" : "-";
        if (edge.Type == EdgeType.Input)
        {
          switch (edge.Direction)
          {
            case Direction.Left:
              symbol = "◄";
              break;
            case Direction.Right:
              symbol = "►";
              break;
            case Direction.Top:
              symbol = "▲";
              break;
            case Direction.Bottom:
              symbol = "▼";
              break;
          }
        }
        Console.ForegroundColor = color;
        Console.SetCursorPosition(edge.Position.X, edge.Position.Y + topOffset);
        Console.Write(symbol);
      }

      ConsoleUtils.PopColor();
    }
  }
}
