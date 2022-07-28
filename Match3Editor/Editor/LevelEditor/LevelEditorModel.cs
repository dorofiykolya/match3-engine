using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Editor.Windows;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Engine.Utils;
using Match3.Providers;
using Match3.Settings;

namespace Match3.Editor.LevelEditor
{
  public class LevelEditorModel
  {
    private readonly Match3Setting _setting;
    private readonly Windows.LevelEditor _levelEditor;

    public int LevelRandomSeed;
    public int GeneratorIteration;
    
    public LevelEditorModel(Match3Setting setting, Windows.LevelEditor levelEditor)
    {
      _setting = setting;
      _levelEditor = levelEditor;
    }

    public TileGridGenerator.Grid GenerateLevel(int width, int height, int[] availableItems)
    {
      var provider = new Match3ProviderFactory();
      var generator = new TileGridGenerator(LevelRandomSeed, provider.Create(_setting));
      var grid = generator.GenerateNext(width, height, availableItems, GeneratorIteration);

      return grid;
    }

    public LevelDescription Serialize()
    {
      return null;
    }

    public LevelDescription Serialize(LevelTileDescription[] items, LevelEdgeDescription[] edges, int swap, LevelRequirementDescription[] requirements)
    {
      var level = new LevelDescription();
      
      level.AvailableItems = _levelEditor.AvailableItems.AvailableItems;
      level.RandomSeed = LevelRandomSeed;
      level.Edges = edges;
      level.Tiles = items;
      level.Requirements = requirements;
      level.Swaps = swap;
      
      return level;
    }
  }
}
