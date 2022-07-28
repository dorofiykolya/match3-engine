using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;
using Match3.Engine.Shareds.Providers;
using Match3Debug;
using Match3Debug.Modules;
using Match3Debug.Providers;

namespace Match3ViewTest
{
  public class ConsoleTestEngine
  {
    public static void Test(int width = 5, int height = 5)
    {
      var engine = CreateEngine(width, height);
      var modules = engine.Configuration.Providers.ModulesProvider.PreTick;
      var debugModule = modules.First(m => m is DebugTileGridOutputModule) as DebugTileGridOutputModule;
      while (!engine.State.IsFinished)
      {
        engine.FastForward(engine.Tick + 1);
      }

      Console.OutputEncoding = Encoding.UTF8;
      debugModule.CurrentStep = 0;
      Draw(debugModule, 0);
      while (true)
      {
        var key = Console.ReadKey(false).Key;
        switch (key)
        {
          case ConsoleKey.Escape:
            return;
          case ConsoleKey.LeftArrow:
            Draw(debugModule, -1);
            break;
          case ConsoleKey.RightArrow:
            Draw(debugModule, 1);
            break;
        }
      }
    }

    private static void Draw(DebugTileGridOutputModule debugModule, int offsetStep)
    {
      Console.Clear();

      debugModule.CurrentStep += offsetStep;
      if (debugModule.CurrentStep < 0) debugModule.CurrentStep = 0;
      else if (debugModule.CurrentStep >= debugModule.Steps.Count)
        debugModule.CurrentStep = debugModule.Steps.Count - 1;

      List<string> lines = new List<string>();

      lines.Add($"step: {debugModule.CurrentStep + 1}/{debugModule.Steps.Count} press left/right arrow button to change step");

      if (debugModule.Steps.Count == 0)
      {
        lines.Add("Empty steps");
        foreach (var line in lines)
        {
          Console.WriteLine(line);
        }
        return;
      }

      var step = debugModule.Steps[debugModule.CurrentStep];
      lines.Add($"tick:{step.Tick}, swaps:{step.Swaps}/{step.MaxSwaps}, score:{step.Score}");

      foreach (var line in lines)
      {
        Console.WriteLine(line);
      }

      ConsoleUtils.PushColor();

      var topOffset = lines.Count + 1;
      var converter = new PositionConverter();

      foreach (var tile in step.Tiles)
      {
        if (tile.Item != null)
        {
          var position = converter.TileToEdgeInternal(tile.Position);
          var color = ConsoleEngineUtils.GetColor(tile.Item.Id, tile.Item.Level);
          Console.ForegroundColor = color;
          Console.SetCursorPosition(position.X, position.Y + topOffset);
          Console.Write("█▓░"[tile.Item.Level]);
        }
      }
      foreach (var edge in step.Edges)
      {
        var color = ConsoleEngineUtils.GetColor(edge.Type);
        var symbol = edge.Orientation == Orientation.Horizontal ? "|" : "-";
        if (edge.Type == EdgeType.Input || edge.Type == EdgeType.TeleportInput || edge.Type == EdgeType.TeleportOutput)
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

    private static Engine CreateEngine(int width, int height)
    {
      var level = TestConfigurationFactory.GetLevelDescription();
      var configuration = new TestConfigurationFactory().Create(EngineEnvironment.DefaultDebugClient, level, 10, width, height);
      configuration.Providers = new SharedProviders
      {
        ItemsProvider = configuration.Providers.ItemsProvider,
        MatchesProvider = configuration.Providers.MatchesProvider,
        ModifierDescriptionProvider = configuration.Providers.ModifierDescriptionProvider,
        CommandsProvider = configuration.Providers.CommandsProvider,
        CombinationActivatorsProvider = configuration.Providers.CombinationActivatorsProvider,
        SpellDescriptionProvider = configuration.Providers.SpellDescriptionProvider,
        SpellTypeActionsProvider = configuration.Providers.SpellTypeActionsProvider,
        SpellCombinationsProvider = configuration.Providers.SpellCombinationsProvider,
        ModulesProvider = new DebugModulesProvider()
      };
      var engine = new Engine(configuration);
      return engine;
    }
  }
}
