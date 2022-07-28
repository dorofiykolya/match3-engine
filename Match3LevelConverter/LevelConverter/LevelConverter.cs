using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Levels;
using Match3.LevelConverter.MagicCrush;
using Match3.LevelConverter.MagicCrush.RulesPerLevel;
using Match3.Settings;

namespace Match3.LevelConverter
{
  public class LevelConverter
  {
    public static LevelDescription Convert(Match3Setting setting, MCLevel magicCrushLevel)
    {
      var level = new LevelDescription();
      Parse(level, magicCrushLevel, setting);
      return level;
    }

    private static void Parse(LevelDescription result, MCLevel mcLevel, Match3Setting setting)
    {
      var rulesPerLevel = mcLevel.rulesPerLevel;

      result.RandomSeed = 0;

      result.Description = rulesPerLevel.description;
      result.Title = rulesPerLevel.title;
      result.Swaps = rulesPerLevel.stepsPerLevel;
      result.Requirements = ParseRequirements(rulesPerLevel, setting);
      result.Tiles = ParseTiles(mcLevel.matrixInfo, setting);
      result.Edges = ParseEdges(mcLevel.matrixInfo, setting);
      result.AvailableItems = ParseAvailableItems(rulesPerLevel.forbiddenSymbols, result.Tiles, setting);
    }

    private static LevelEdgeDescription[] ParseEdges(MCMatrixInfo matrixInfo, Match3Setting setting)
    {
      var converter = new PositionConverter();
      var list = new List<LevelEdgeDescription>();
      for (int x = 0; x < matrixInfo.matrixCellIndexes.Length; x++)
      {
        for (int yIndex = 0; yIndex < matrixInfo.matrixCellIndexes[x].Length; yIndex++)
        {
          var y = matrixInfo.matrixCellIndexes[x][yIndex];

          var position = new Point(x, y);

          Point? edgePosition = null;
          EdgeType edgeType = EdgeType.None;
          Direction direction = Direction.None;
          int index = 0;
          int[] additianalItems = null;
          var borderIndex = matrixInfo.backbroundBodrersIndexes[x][yIndex];
          if (borderIndex != 0)
          {
            switch (borderIndex)
            {
              case 1: // left
                edgePosition = converter.TileToEdge(position, Direction.Left);
                break;
              case 2: // right
                edgePosition = converter.TileToEdge(position, Direction.Right);
                break;
              case 3: // top
                edgePosition = converter.TileToEdge(position, Direction.Top);
                break;
              case 4: // bottom
                edgePosition = converter.TileToEdge(position, Direction.Bottom);
                break;
            }
            if (edgePosition != null)
            {
              var edge = new LevelEdgeDescription();
              edge.Position = edgePosition.Value;
              edge.Type = EdgeType.Lock;
              list.Add(edge);
            }
          }

          var portalIn = matrixInfo.portalInIndexes[x][yIndex];
          if (portalIn == 0)
          {
            var outPortal = matrixInfo.portalOutIndexes[x][yIndex];
            if (outPortal == 1)
            {
              edgePosition = converter.TileToEdge(position, Direction.Top);
              edgeType = EdgeType.Input;
              direction = Direction.Bottom;

              if (matrixInfo.additionalMatrix != null && matrixInfo.additionalMatrix.Length > x)
              {
                additianalItems = matrixInfo.additionalMatrix[x];
              }
            }
            else if (outPortal > 1)
            {
              edgePosition = converter.TileToEdge(position, Direction.Top);
              edgeType = EdgeType.TeleportOutput;
              direction = Direction.Bottom;
              index = outPortal;
            }
            else
            {
              var box = matrixInfo.boxIndexes[x][yIndex];
              if (box != 0)
              {
                edgePosition = converter.TileToEdge(position, Direction.Bottom);
                edgeType = EdgeType.Output;
                direction = Direction.Top;
              }
            }
          }
          else
          {
            index = portalIn;
            edgePosition = converter.TileToEdge(position, Direction.Bottom);
            edgeType = EdgeType.TeleportInput;
            direction = Direction.Top;
          }

          if (edgePosition != null)
          {
            var edge = new LevelEdgeDescription();
            edge.Position = edgePosition.Value;
            edge.Index = (byte)index;
            edge.Type = edgeType;
            edge.Direction = direction;
            edge.Queue = additianalItems != null ? additianalItems.Select(i => new Item(i, LevelId.L0)).ToArray() : null;
            list.Add(edge);
          }
        }
      }
      return list.ToArray();
    }

    private static LevelTileDescription[] ParseTiles(MCMatrixInfo matrixInfo, Match3Setting setting)
    {
      var list = new List<LevelTileDescription>();
      for (int x = 0; x < matrixInfo.matrixCellIndexes.Length; x++)
      {
        for (int yIndex = 0; yIndex < matrixInfo.matrixCellIndexes[x].Length; yIndex++)
        {
          var y = matrixInfo.matrixCellIndexes[x][yIndex];

          var tile = new LevelTileDescription();

          tile.Position = new Point(x, y);
          tile.Direction = Direction.Bottom;
          tile.Type = TileType.Movable;
          var itemId = ConvertItemId(matrixInfo.matrixSymbolIndexes[x][yIndex]);
          var itemLevel = ConvertItemLevel(matrixInfo.matrixSymbolIndexes[x][yIndex], matrixInfo.matrixSymbolLevels[x][yIndex]);
          if (itemId != ItemId.Empty)
          {
            if (itemLevel >= LevelId.L2)
            {
              itemId = ItemId.Universal;
              itemLevel = 0;
            }
            tile.Item = new Item(itemId, itemLevel);
          }

          var modifiers = new List<Modifier>();
          if (matrixInfo.lockedSymbolsIndexes[x][yIndex] != 0)
          {
            modifiers.Add(new Modifier
            {
              Id = setting.Modifiers.First(m => m.Type == ModifierType.Armor).Id,
              Level = matrixInfo.lockedSymbolsIndexes[x][yIndex]
            });
          }
          if (matrixInfo.backgroundLockedCellIndexes[x][yIndex] != 0)
          {
            modifiers.Add(new Modifier
            {
              Id = setting.Modifiers.First(m => m.Type == ModifierType.Substrate).Id,
              Level = matrixInfo.backgroundLockedCellIndexes[x][yIndex]
            });
          }

          tile.Modifiers = modifiers.ToArray();


          list.Add(tile);
        }
      }
      return list.ToArray();
    }

    private static int ConvertItemLevel(int id, int level)
    {
      switch ((MCItem)id)
      {
        case MCItem.level2_green:
        case MCItem.level2_yellow:
        case MCItem.level2_white:
        case MCItem.level2_blue:
        case MCItem.level2_violet:
        case MCItem.level2_red: return LevelId.L1;

        case MCItem.level3:
        case MCItem.unique:
        case MCItem.Artifact:
        case MCItem.Empty: return LevelId.L0;
      }
      return level - 1;
    }

    private static int ConvertItemId(int id)
    {
      switch ((MCItem)id)
      {
        case MCItem.green: return ItemId.Green;
        case MCItem.yellow: return ItemId.Yellow;
        case MCItem.white: return ItemId.White;
        case MCItem.blue: return ItemId.Blue;
        case MCItem.violet: return ItemId.Violet;
        case MCItem.red: return ItemId.Red;

        case MCItem.level2_green: return ItemId.Green;
        case MCItem.level2_yellow: return ItemId.Yellow;
        case MCItem.level2_white: return ItemId.White;
        case MCItem.level2_blue: return ItemId.Blue;
        case MCItem.level2_violet: return ItemId.Violet;
        case MCItem.level2_red: return ItemId.Red;

        case MCItem.level3: return ItemId.Universal;

        case MCItem.unique: return ItemId.Artifact;

        case MCItem.Artifact: return ItemId.Artifact;
        case MCItem.Empty: return ItemId.Empty;
        default:
          throw new ArgumentException("not valid item id: " + id);
      }
    }

    private static LevelRequirementDescription[] ParseRequirements(MCRulesPerLevel rules, Match3Setting setting)
    {
      var result = new List<LevelRequirementDescription>();

      AddItemRequirement(result, ItemId.Blue, LevelId.L0, rules.tasksPerLevel.blue);
      AddItemRequirement(result, ItemId.White, LevelId.L0, rules.tasksPerLevel.white);
      AddItemRequirement(result, ItemId.Green, LevelId.L0, rules.tasksPerLevel.green);
      AddItemRequirement(result, ItemId.Red, LevelId.L0, rules.tasksPerLevel.red);
      AddItemRequirement(result, ItemId.Violet, LevelId.L0, rules.tasksPerLevel.violet);
      AddItemRequirement(result, ItemId.Yellow, LevelId.L0, rules.tasksPerLevel.yellow);

      AddItemRequirement(result, ItemId.Blue, LevelId.L1, rules.tasksPerLevel.level2_blue);
      AddItemRequirement(result, ItemId.White, LevelId.L1, rules.tasksPerLevel.level2_white);
      AddItemRequirement(result, ItemId.Green, LevelId.L1, rules.tasksPerLevel.level2_green);
      AddItemRequirement(result, ItemId.Red, LevelId.L1, rules.tasksPerLevel.level2_red);
      AddItemRequirement(result, ItemId.Violet, LevelId.L1, rules.tasksPerLevel.level2_violet);
      AddItemRequirement(result, ItemId.Yellow, LevelId.L1, rules.tasksPerLevel.level2_yellow);

      AddItemRequirement(result, ItemId.Universal, LevelId.L0, rules.tasksPerLevel.level3);

      AddItemRequirement(result, ItemId.Artifact, LevelId.L0, rules.tasksPerLevel.unique);

      AddRequirement(result, LevelRequirementType.Modifier, setting.Modifiers.First(m => m.Type == ModifierType.Armor).Id, 0, rules.tasksPerLevel.locked_front);
      AddRequirement(result, LevelRequirementType.Modifier, setting.Modifiers.First(m => m.Type == ModifierType.Substrate).Id, 0, rules.tasksPerLevel.locked_back);
      AddRequirement(result, LevelRequirementType.UseSpell, rules.useBoostWithId, 0, rules.tasksPerLevel.useBoost);

      if (rules.scoresRequired == null || rules.scoresRequired.Length == 0)
      {
        AddRequirement(result, LevelRequirementType.Stars, 0, 1, 0);
        AddRequirement(result, LevelRequirementType.Stars, 0, 2, 0);
        AddRequirement(result, LevelRequirementType.Stars, 0, 3, 0);
      }
      else if (rules.scoresRequired.Length == 1)
      {
        AddRequirement(result, LevelRequirementType.Stars, 0, 1, 0);
        AddRequirement(result, LevelRequirementType.Stars, 0, 2, 0);
        AddRequirement(result, LevelRequirementType.Stars, 0, 3, rules.scoresRequired[0]);
      }
      else if (rules.scoresRequired.Length == 2)
      {
        AddRequirement(result, LevelRequirementType.Stars, 0, 1, 0);
        AddRequirement(result, LevelRequirementType.Stars, 0, 2, rules.scoresRequired[0]);
        AddRequirement(result, LevelRequirementType.Stars, 0, 3, rules.scoresRequired[1]);
      }
      else if (rules.scoresRequired.Length == 3)
      {
        AddRequirement(result, LevelRequirementType.Stars, 0, 1, rules.scoresRequired[0]);
        AddRequirement(result, LevelRequirementType.Stars, 0, 2, rules.scoresRequired[1]);
        AddRequirement(result, LevelRequirementType.Stars, 0, 3, rules.scoresRequired[2]);
      }

      return result.ToArray();
    }

    private static int[] ParseAvailableItems(MCForbiddenSymbols forbiddenSymbols, LevelTileDescription[] tiles,
      Match3Setting setting)
    {
      var result = new List<int>();

      if (forbiddenSymbols != null)
      {
        if (forbiddenSymbols.blue) result.Add(ItemId.Blue);
        if (forbiddenSymbols.green) result.Add(ItemId.Green);
        if (forbiddenSymbols.red) result.Add(ItemId.Red);
        if (forbiddenSymbols.violet) result.Add(ItemId.Violet);
        if (forbiddenSymbols.white) result.Add(ItemId.White);
        if (forbiddenSymbols.yellow) result.Add(ItemId.Yellow);
      }
      else
      {
        foreach (var tile in tiles)
        {
          if (tile.Item != null)
          {
            var it = setting.Items.FirstOrDefault(i => i.Id == tile.Item.Id);
            if (it != null)
            {
              if (it.Type == ItemType.Cell)
              {
                result.Add(tile.Item.Id);
              }
            }
          }
        }

        result = result.Distinct().ToList();
      }

      return result.ToArray();
    }

    private static void AddItemRequirement(List<LevelRequirementDescription> list, int id, int level, int value)
    {
      if (value > 0)
      {
        list.Add(CreateRequirement(LevelRequirementType.CollectItem, id, level, value));
      }
    }

    private static void AddRequirement(List<LevelRequirementDescription> list, LevelRequirementType type, int id, int level, int value)
    {
      if (value > 0)
      {
        list.Add(CreateRequirement(type, id, level, value));
      }
    }

    private static LevelRequirementDescription CreateRequirement(LevelRequirementType type, int id, int level, int value)
    {
      return new LevelRequirementDescription
      {
        Type = type,
        Id = id,
        Level = level,
        Value = value
      };
    }

    /// <summary>
    /// copy from MagicCrush
    /// </summary>
    public enum MCItem
    {
      none = 0,
      green = 1,
      yellow = 2,
      white = 3,
      blue = 4,
      violet = 5,
      red = 6,
      level2_green = 7,
      level2_yellow = 8,
      level2_white = 9,
      level2_blue = 10,
      level2_violet = 11,
      level2_red = 12,
      level3 = 13,
      unique = 14,
      locked_front = 15,
      locked_back = 16,
      useBoost = 17,
      Empty = -2,
      Artifact = -3
    }
  }
}
