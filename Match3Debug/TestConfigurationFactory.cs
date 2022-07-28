using System;
using Match3.Engine;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Descriptions.SpellCombinations;
using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Levels;
using Match3.Engine.Providers;
using Match3.Engine.Shareds.Providers;
using Match3.Engine.Utils;

namespace Match3Debug
{
  public class TestConfigurationFactory
  {
    public TestConfigurationFactory()
    {

    }

    public Configuration Create(EngineEnvironment environment, LevelDescription level, int availableSwaps, int width, int height)
    {
      var configuration = new Configuration(environment)
      {
        Providers = CreateProviders(availableSwaps, width, height),
        LevelDescription = level,
        MaxTicks = 20,
      };

      return configuration;
    }

    public IEngineProviders CreateProviders(int availableSwaps, int width, int height)
    {
      return new SharedProviders
      {
        ItemsProvider = CreateItemsProvider(),
        MatchesProvider = CreateMatchProvder(),
        CommandsProvider = CreateCommandsProvider(),
        ModulesProvider = CreateModuleProvider(),
        CombinationActivatorsProvider = CreateCombinationActivatorsProvider(),
        ModifierDescriptionProvider = CreateModifierDescriptionProvider(),
        SpellDescriptionProvider = CreateSpellDescriptionProvider(),
        SpellTypeActionsProvider = CreateSpellTypeActionsProvider(),
        SpellCombinationsProvider = CreateSpellCombinationsProvider()
      };
    }

    private ISpellCombinationsProvider CreateSpellCombinationsProvider()
    {
      return new SharedSpellCombinationsProvider(new SpellCombinationDescription[0]);
    }

    private ISpellTypeActionsProvider CreateSpellTypeActionsProvider()
    {
      return new SharedSpellTypeActionProvider();
    }

    private ISpellDescriptionProvider CreateSpellDescriptionProvider()
    {
      return new SharedSpellDescriptionProvider(new[]
      {
        new SpellDescription
        {
          Id = 1,
          Type = SpellType.ChangeItem,
          Levels = new []
          {
            new SpellLevelDescription { Value = 2 },
            new SpellLevelDescription { Value = 3 },
            new SpellLevelDescription { Value = 4 },
            new SpellLevelDescription { Value = 5 },
            new SpellLevelDescription { Value = 6 },
            new SpellLevelDescription { Value = 7 },
          }
        },
        new SpellDescription
        {
          Id = 2,
          Type = SpellType.MakeBonusItem,
          Levels = new []
          {
            new SpellLevelDescription{ Value = 1 },
            new SpellLevelDescription{ Value = 2 },
            new SpellLevelDescription{ Value = 3 },
            new SpellLevelDescription{ Value = 4 },
            new SpellLevelDescription{ Value = 5 },
            new SpellLevelDescription{ Value = 6 },
          }
        },
        new SpellDescription
        {
          Id = 3,
          Type = SpellType.DestroySomeItem,
          Levels = new []
          {
            new SpellLevelDescription{ Value = 1 },
            new SpellLevelDescription{ Value = 2 },
            new SpellLevelDescription{ Value = 3 },
            new SpellLevelDescription{ Value = 4 },
            new SpellLevelDescription{ Value = 5 },
            new SpellLevelDescription{ Value = 6 },
          }
        },
        new SpellDescription
        {
          Id = 4,
          Type = SpellType.RandomDestory,
          Levels = new []
          {
            new SpellLevelDescription { Value = 1 },
            new SpellLevelDescription { Value = 2 },
            new SpellLevelDescription { Value = 3 },
            new SpellLevelDescription { Value = 4 },
            new SpellLevelDescription { Value = 5 },
            new SpellLevelDescription { Value = 6 },
          }
        },
        new SpellDescription
        {
          Id = 5,
          Type = SpellType.Splash,
          Levels = new []
          {
            new SpellLevelDescription { Value = 1 },
            new SpellLevelDescription { Value = 2 },
            new SpellLevelDescription { Value = 3 },
            new SpellLevelDescription { Value = 4 },
            new SpellLevelDescription { Value = 5 },
            new SpellLevelDescription { Value = 6 },
          }
        },
        new SpellDescription
        {
          Id = 6,
          Type = SpellType.ChainDestroy,
          Levels = new []
          {
            new SpellLevelDescription { Value = 1 },
            new SpellLevelDescription { Value = 2 },
            new SpellLevelDescription { Value = 3 },
            new SpellLevelDescription { Value = 4 },
            new SpellLevelDescription { Value = 5 },
            new SpellLevelDescription { Value = 6 },
          }
        }
      });
    }

    private IModifierDescriptionProvider CreateModifierDescriptionProvider()
    {
      return new SharedModifierDescriptionProvider(new[]
      {
        new ModifierDescription{Id = 1, Type = ModifierType.Armor},
        new ModifierDescription{Id = 2, Type = ModifierType.Substrate},
        new ModifierDescription{Id = 3, Type = ModifierType.Box}
      });
    }

    private ICombinationActivatorsProvider CreateCombinationActivatorsProvider()
    {
      return new SharedCombinationActivatorsProvider();
    }

    private SharedModulesProvider CreateModuleProvider()
    {
      return new SharedModulesProvider();
    }

    private SharedCommandsProvider CreateCommandsProvider()
    {
      return new SharedCommandsProvider();
    }

    public SharedMatchesProvider CreateMatchProvder()
    {
      return new SharedMatchesProvider();
    }

    public SharedItemDescriptionProvider CreateItemsProvider()
    {
      return new SharedItemDescriptionProvider(new[]
      {
        new ItemDescription
        {
          Id = 1,
          Levels = new[]
          {
            new ItemLevelDescription {Score = 1},
            new ItemLevelDescription {Score = 2}
          },
          Type = ItemType.Cell
        },
        new ItemDescription
        {
          Id = 2,
          Levels = new[]
          {
            new ItemLevelDescription {Score = 1},
            new ItemLevelDescription {Score = 2}
          },
          Type = ItemType.Cell
        },
        new ItemDescription
        {
          Id = 3,
          Levels = new[]
          {
            new ItemLevelDescription {Score = 1},
            new ItemLevelDescription {Score = 2}
          },
          Type = ItemType.Cell
        },
      });
    }

    public static LevelDescription GetLevelDescription(int width = 10, int height = 10, int availableSwaps = 10)
    {
      var random = new EngineRandom(0);

      Func<int, int> generateInputX = (i) =>
      {
        var value = random.Next(i);
        if (value % 2 == 0)
        {
          value++;
        }
        if (value >= width) value -= 2;
        return value;
      };

      var result = new LevelDescription();
      result.Swaps = availableSwaps;
      result.AvailableItems = new[] { 1, 2, 3 };
      result.Requirements = new LevelRequirementDescription[]
      {
        new LevelRequirementDescription
        {
          Id = 1,
          Level = 0,
          Type = LevelRequirementType.CollectItem,
          Value = random.Next(availableSwaps * 2)
        },
        new LevelRequirementDescription
        {
          Id = 2,
          Level = 0,
          Type = LevelRequirementType.CollectItem,
          Value = random.Next(availableSwaps * 2)
        },
        new LevelRequirementDescription
        {
          Id = 3,
          Level = 0,
          Type = LevelRequirementType.CollectItem,
          Value = random.Next(availableSwaps * 2)
        }
      };
      result.Tiles = new LevelTileDescription[width * height];
      result.Edges = new LevelEdgeDescription[]
      {
        new LevelEdgeDescription
        {
          Position = new Point(generateInputX(width),0),
          Type = EdgeType.Input,
          Direction = Direction.Bottom
        },
        new LevelEdgeDescription()
        {
          Position = new Point(generateInputX(width),0),
          Type = EdgeType.Input,
          Direction = Direction.Bottom
        },
        new LevelEdgeDescription()
        {
          Position = new Point(generateInputX(width),0),
          Type = EdgeType.Input,
          Direction = Direction.Bottom
        }
      };

      for (int x = 0, i = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++, i++)
        {
          result.Tiles[i] = new LevelTileDescription
          {
            Direction = Direction.Bottom,
            Item = random.Next(2) > 0 ? new Item
            {
              Id = random.Next(1, 4),
              Level = 0//random.Next(0, 2)
            } : null,
            Position = new Point(x, y),
            Modifiers = new Modifier[0]
          };
        }
      }

      return result;
    }
  }
}
