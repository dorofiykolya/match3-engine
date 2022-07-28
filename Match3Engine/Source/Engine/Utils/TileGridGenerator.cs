using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Engine.Matches;
using Match3.Engine.Providers;

namespace Match3.Engine.Utils
{
  /// <summary>
  /// Генератор Match3 уровня
  /// </summary>
  public class TileGridGenerator
  {
    private readonly int _randomSeed;
    private readonly IEngineProviders _providers;

    public TileGridGenerator(int randomSeed, IEngineProviders providers)
    {
      _randomSeed = randomSeed;
      _providers = providers;
    }

    /// <summary>
    /// сгенерировать уровень
    /// </summary>
    /// <param name="width">ширина уровня</param>
    /// <param name="height">высота уровня</param>
    /// <param name="availableItems">доступные предметы</param>
    /// <param name="iteration">вариант уровня</param>
    /// <returns></returns>
    public Grid GenerateNext(int width, int height, int[] availableItems, int iteration = 0)
    {
      if (availableItems == null) throw new ArgumentNullException("availableItems == null");
      if (availableItems.Length == 0) throw new ArgumentException("availableItems.Length == 0");
      if (iteration < 0) throw new ArgumentException("iteration must bee >= 0");
      if (availableItems.Any(i => _providers.ItemsProvider.Get(i) == null)) throw new ArgumentException("item description not found");

      var random = new EngineRandom(_randomSeed);
      while (iteration-- + 1 > 0) random.Next();

      var converter = new PositionConverter();

      var tiles = new List<LevelTileDescription>();
      var edges = new List<LevelEdgeDescription>();
      var warnings = new List<string>();

      var result = new Grid(tiles, edges, warnings, width, height);

      var tileMap = new Dictionary<Point, LevelTileDescription>();
      var edgeMap = new Dictionary<Point, LevelEdgeDescription>();

      for (int x = 0; x < width; x++)
      {
        edges.Add(new LevelEdgeDescription
        {
          Direction = Direction.Bottom,
          Type = EdgeType.Input,
          Position = converter.TileToEdge(new Point(x, 0), Direction.Top),
          Queue = new Item[0]
        });
      }

      foreach (var edge in edges)
      {
        edgeMap[edge.Position] = edge;
      }

      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          var isItemChosen = true;
          var itemId = 0;
          var itemChosenIteration = availableItems.Length;
          do
          {
            isItemChosen = true;
            var item = _providers.ItemsProvider.Get(availableItems[random.Next(availableItems.Length)]);
            itemId = item.Id;
            if (x >= 2 && y >= 2) isItemChosen = !ItemAboveFormsChain(tileMap, x, y, itemId) && !ItemAtLeftFormsChain(tileMap, x, y, itemId);

            if (--itemChosenIteration <= 0)
            {
              warnings.Add("достигнут лимит операций, возможно, добавьте к-во доступных предетов на уровне");
              isItemChosen = true;
              int i = 0;
              for (; i < availableItems.Length; i++)
              {
                itemId = availableItems[i];
                if (x >= 2) isItemChosen = isItemChosen && !ItemAtLeftFormsChain(tileMap, x, y, availableItems[i]);
                if (y >= 2) isItemChosen = isItemChosen && !ItemAboveFormsChain(tileMap, x, y, availableItems[i]);
              }
              if (!isItemChosen)
              {
                isItemChosen = true;
                warnings.Add("не получилось создать поле без совпадений");
              }
            }
          } while (!isItemChosen);
          CreateNewItemTo(tileMap, x, y, itemId);
        }
      }

      if (warnings.Count != 0)
      {
        List<List<Point>> matches;
        if ((matches = FindMatches(tileMap, edgeMap, availableItems)) != null && matches.Count != 0)
        {
          try
          {
            BalanceField(tileMap, edgeMap, random, availableItems);
            warnings.Add("была произведена балансировка");
          }
          catch (Exception e)
          {
            warnings.Add(string.Format("ошибка при балансировке, слишком много операций: \"{0}\"", e.Message));
          }
        }
      }

      foreach (var tile in tileMap)
      {
        tiles.Add(tile.Value);
      }

      return result;
    }

    private void CreateNewItemTo(Dictionary<Point, LevelTileDescription> map, int x, int y, int itemId)
    {
      map[new Point(x, y)] = new LevelTileDescription
      {
        Direction = Direction.Bottom,
        Type = TileType.Movable,
        Position = new Point(x, y),
        Item = new Item { Id = itemId },
        Modifiers = new Modifier[0]
      };
    }

    private List<List<Point>> FindMatches(Dictionary<Point, LevelTileDescription> tiles, Dictionary<Point, LevelEdgeDescription> edges, int[] availableItems)
    {
      var grid = new TileGrid(null, new LevelDescription
      {
        Tiles = tiles.Values.ToArray(),
        Edges = edges.Values.ToArray(),
        AvailableItems = availableItems,
        RandomSeed = 0,
        Requirements = new LevelRequirementDescription[0]
      }, _providers, EngineEnvironment.DefaultDebugClient);
      var result = new MatchCombinationsResult();
      foreach (var tileDescription in tiles)
      {
        _providers.MatchesProvider.Match(tileDescription.Key, grid, result);
      }
      return result.Combinations.Select(p => p.Combination.ToList()).ToList();
    }

    private void BalanceField(Dictionary<Point, LevelTileDescription> map, Dictionary<Point, LevelEdgeDescription> edges, EngineRandom random, int[] availableItems)
    {
      // Пока на поле имеются матчи, мы будем продолжать их разбивать.
      // Нам приходится повторять итерации, так как перестановки могут 
      // образовать новые матчи.
      List<List<Point>> matches;
      var maxIteration = 1000;
      var iteration = 0;
      while ((matches = FindMatches(map, edges, availableItems)) != null && matches.Count > 0)
      {
        foreach (var match in matches)
        {
          if (iteration >= maxIteration) throw new InvalidOperationException(string.Format("перевышено {0} операций", maxIteration));
          ++iteration;
          BreakMatch(match, map, random);
        }
      }
    }

    private void BreakMatch(List<Point> match, Dictionary<Point, LevelTileDescription> map, EngineRandom random)
    {
      // Если длина матча больше 3-х, то он разбивается на мелкие цепочки по 3 элемента.
      if (match.Count > 3)
      {
        int chainsCount = (int)Math.Ceiling(match.Count / 3.0);
        int i;
        for (i = 0; i < chainsCount; i++)
        {
          BreakMatch(CutChainPart(match), map, random);
        }
        return;
      }

      foreach (var position in match)
      {
        if (TryToSwapWithNeighbours(position, map, random)) return;
      }
    }

    private List<Point> CutChainPart(List<Point> match)
    {
      if (match.Count <= 3) return match;
      return match.Take(3).ToList();
    }

    private bool TryToSwapWithNeighbours(Point position, Dictionary<Point, LevelTileDescription> map, EngineRandom random)
    {
      int targetId = map[position].Item.Id;

      /**
       * Мы определяем случайный сдвиг для выбора направления обмена,
       * для того, чтобы не создавать ситуацию бесконечных перестановок
       * обменивая два элемента таким образом, что при обеих позициях 
       * один из них всё равно образует матч.
       **/

      int directionIndexOffset = random.Next();
      int length = Point.Directions.Length;

      var temp = new Point();

      for (var i = 0; i < length; i++)
      {
        var direction = Point.Directions[(i + directionIndexOffset) % length];

        temp.X = position.X + direction.X;
        temp.Y = position.Y + direction.Y;

        if (map.ContainsKey(new Point(temp.X, temp.Y)))
        {
          var neighbourId = map[new Point(temp.X, temp.Y)].Item.Id;

          // Элемент для перестановки был найден, производим обмен
          // и прекращаем поиск.
          if (targetId != neighbourId)
          {
            Swap(map, position, temp);
            return true;
          }
        }
      }

      // Элемент для перестановки не был найден
      return false;
    }

    private void Swap(Dictionary<Point, LevelTileDescription> map, Point first, Point second)
    {
      var f = map[first];
      map[first] = map[second];
      map[second] = f;
      map[first].Position = first;
      map[second].Position = second;
    }

    /// <summary>
    ///Два элемента слева проверяются по следующему алгоритму
    ///
    ///- Если элемент отсутствует - цепочки быть не может
    ///- Если элемент имеет отличный id от item - цепочки быть не может
    ///
    ///Так как используется сокращённая логика, то первое же неудовлетворительное
    ///условие прервет проверку и вернёт отрицательный результат.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    private bool ItemAtLeftFormsChain(Dictionary<Point, LevelTileDescription> map, int x, int y, int itemId)
    {
      return isEquals(map, x - 1, y, itemId) && isEquals(map, x - 2, y, itemId);
    }

    /// <summary>
    ///Два элемента слева проверяются по следующему алгоритму
    ///
    ///- Если элемент отсутствует - цепочки быть не может
    ///- Если элемент имеет отличный id от item - цепочки быть не может
    ///
    ///Так как используется сокращённая логика, то первое же неудовлетворительное
    ///условие прервет проверку и вернёт отрицательный результат.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    private bool ItemAboveFormsChain(Dictionary<Point, LevelTileDescription> map, int x, int y, int itemId)
    {
      return isEquals(map, x, y - 1, itemId) || isEquals(map, x, y - 2, itemId);
    }

    private bool isEquals(Dictionary<Point, LevelTileDescription> map, int x, int y, int itemId)
    {
      return map.ContainsKey(new Point(x, y)) && map[new Point(x, y)].Item.Id == itemId;
    }

    public class Grid
    {
      public Grid(IEnumerable<LevelTileDescription> tiles, IEnumerable<LevelEdgeDescription> edges, IEnumerable<string> warningMessages, int width, int height)
      {
        Tiles = tiles;
        Edges = edges;
        WarningMessages = warningMessages;
        Width = width;
        Height = height;
      }

      public IEnumerable<LevelTileDescription> Tiles { get; private set; }
      public IEnumerable<LevelEdgeDescription> Edges { get; private set; }
      public IEnumerable<string> WarningMessages { get; private set; }
      public int Width { get; private set; }
      public int Height { get; private set; }

      public LevelDescription ToLevelDescription()
      {
        var level = new LevelDescription
        {
          RandomSeed = 0,
          Swaps = 0,
          AvailableItems = null,
          Requirements = null,
          Edges = Edges.ToArray(),
          Tiles = Tiles.ToArray()
        };

        return level;
      }
    }

  }
}
