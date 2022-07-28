using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Providers;
using Match3.Engine.Utils;

namespace Match3.Engine.Levels
{
  /// <summary>
  /// Данные игрового поля
  /// </summary>
  public class TileGrid : ITileGridProvider
  {
    private readonly IEngineNextRandom _engineRandomState;
    private readonly IMatchesProvider _matchCombinations;
    private readonly IModifierDescriptionProvider _modifierDescriptionProvider;
    private readonly EngineEnvironment _environment;
    private readonly Dictionary<Point, Tile> _tileMap;
    private readonly Dictionary<Point, Edge> _edgeMap;
    private readonly List<Edge> _inputs;
    private readonly PositionConverter _positionConverter;
    private readonly TileItemGenerator _itemGenerator;
    private readonly HashSet<Tile> _endTiles;
    private readonly Dictionary<EdgeIndex, Edge> _edgeByIndex;
    private readonly HashSet<Tile> _empty;
    private readonly int _tileIndex;
    private readonly int _edgeIndex;
    private readonly Bounds _bounds;

    public TileGrid(IEngineNextRandom engineRandomState, LevelDescription levelDescription, IEngineProviders providers, EngineEnvironment environment)
    {
      _engineRandomState = engineRandomState;
      _matchCombinations = providers.MatchesProvider;
      _modifierDescriptionProvider = providers.ModifierDescriptionProvider;
      _environment = environment;

      _edgeByIndex = new Dictionary<EdgeIndex, Edge>();
      _endTiles = new HashSet<Tile>();
      _tileMap = new Dictionary<Point, Tile>();
      _edgeMap = new Dictionary<Point, Edge>();
      _inputs = new List<Edge>();
      _empty = new HashSet<Tile>();
      _positionConverter = new PositionConverter();
      _itemGenerator = new TileItemGenerator(this, levelDescription, engineRandomState, providers, environment);

      //обновляем область поля
      _bounds = BoundsUtils.Calculate(levelDescription.Tiles);

      // устанавливаем ячейки
      foreach (var tileDescription in levelDescription.Tiles)
      {
        var tile = new Tile(this, tileDescription, providers, ++_tileIndex);
        _tileMap[tileDescription.Position] = tile;
      }

      // устанавливаем границы для ячеек
      foreach (var edgeDescription in levelDescription.Edges)
      {
        AddEdge(new Edge(this, Converter, edgeDescription, ++_edgeIndex));
      }

      // устанавливаем (по умолчанию) границы для ячеек, которых нет в инфо о уровне
      foreach (var tile in Tiles)
      {
        if (tile.LeftEdge == null) AddEdge(new Edge(this, Converter, Converter.TileToEdge(tile.Position, Direction.Left)));
        if (tile.RightEdge == null) AddEdge(new Edge(this, Converter, Converter.TileToEdge(tile.Position, Direction.Right)));
        if (tile.TopEdge == null) AddEdge(new Edge(this, Converter, Converter.TileToEdge(tile.Position, Direction.Top)));
        if (tile.BottomEdge == null) AddEdge(new Edge(this, Converter, Converter.TileToEdge(tile.Position, Direction.Bottom)));
      }

      // добавление с список пустых ячеек
      foreach (var tile in Tiles)
      {
        if (tile.IsEmpty)
        {
          AddToEmpty(tile);
        }
      }
      //расчитываем последние ячейки по пути
      //foreach (var tile in Tiles)
      //{
      //  var current = tile;
      //  var next = tile.Next;
      //  while (next != null)
      //  {
      //    current = next;
      //    next = next.Next;
      //  }
      //  _endTiles.Add(current);
      //}
    }

    /// <summary>
    /// генератор новых предметов
    /// </summary>
    public TileItemGenerator Generator
    {
      get { return _itemGenerator; }
    }

    /// <summary>
    /// конвертор координат ячейка-граница
    /// </summary>
    public PositionConverter Converter
    {
      get { return _positionConverter; }
    }

    /// <summary>
    /// границы поля
    /// </summary>
    public Bounds Bounds
    {
      get { return _bounds; }
    }

    /// <summary>
    /// к-во тайлов
    /// </summary>
    public int TileCount { get { return _tileMap.Count; } }

    /// <summary>
    /// набоя ячеек
    /// </summary>
    public IEnumerable<Tile> Tiles
    {
      get { return _tileMap.Values; }
    }

    /// <summary>
    /// последние ячейки в пути
    /// </summary>
    //public IEnumerable<Tile> EndTiles
    //{
    //  get { return _endTiles; }
    //}

    /// <summary>
    /// набор границ для ячеек
    /// </summary>
    public IEnumerable<Edge> Edges
    {
      get { return _edgeMap.Values; }
    }

    /// <summary>
    /// границы с которых генерируются ячейки
    /// </summary>
    public IEnumerable<Edge> Inputs
    {
      get { return _inputs; }
    }

    /// <summary>
    /// получить ячейку по координате ячейки (в пространстве координат ячеек)
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public Tile GetTile(Point tilePosition)
    {
      Tile result;
      _tileMap.TryGetValue(tilePosition, out result);
      return result;
    }

    /// <summary>
    /// получить ячейку, при перемещении
    /// </summary>
    /// <returns>результат</returns>
    /// <param name="swap">ячейки для перемещения</param>
    /// <param name="tilePosition">позиция ячейки</param>
    public Tile GetTileBySwap(Swap swap, Point tilePosition)
    {
      //if (CanSwap(swap)) throw new InvalidOperationException("нельзя перемещать данные ячейки:" + swap);
      if (tilePosition == swap.First) return GetTile(swap.Second);
      if (tilePosition == swap.Second) return GetTile(swap.First);
      return GetTile(tilePosition);
    }

    /// <summary>
    /// получить границы ячейки по координатам границы (в пространсве координат границ)
    /// </summary>
    /// <param name="edgePosition"></param>
    /// <returns></returns>
    public Edge GetEdge(Point edgePosition)
    {
      Edge result;
      _edgeMap.TryGetValue(edgePosition, out result);
      return result;
    }

    /// <summary>
    /// переместить ячейки
    /// </summary>
    /// <returns>The swap.</returns>
    /// <param name="swap">Swap.</param>
    public void Swap(Swap swap)
    {
      if (!CanSwap(swap))
        throw new InvalidOperationException("нельзя перемещать данные ячейки:" + swap);

      var first = GetTile(swap.First);
      var second = GetTile(swap.Second);

      first.SetNextItem(second.Item);
      second.SetNextItem(first.Item);

      first.ApplyNextItem();
      second.ApplyNextItem();
    }

    /// <summary>
    /// получить грань относительно ячейки
    /// </summary>
    /// <param name="tilePosition">позиция ячейки (в пространстве координат ячеек)</param>
    /// <param name="direction">направление</param>
    /// <returns></returns>
    public Edge GetEdgeByTile(Point tilePosition, Direction direction)
    {
      return GetEdge(Converter.TileToEdge(tilePosition, direction));
    }

    /// <summary>
    /// получить ячейку относительно грани
    /// </summary>
    /// <param name="edgePosition">позиция грани (в пространсве координат границ)</param>
    /// <param name="direction">направление</param>
    /// <returns></returns>
    public Tile GetTileByEdge(Point edgePosition, Direction direction)
    {
      return GetTile(Converter.EdgeToTile(edgePosition, direction));
    }

    /// <summary>
    /// получить следующую ячейку по направлению
    /// </summary>
    /// <param name="tilePosition">позиция ячейки (в пространстве координат ячеек)</param>
    /// <param name="direction">направление</param>
    /// <returns></returns>
    public Tile GetTile(Point tilePosition, Direction direction)
    {
      return GetTile(tilePosition + Point.Direction(direction));
    }

    /// <summary>
    /// получить грань по ее индексу и типу (индекс задается для телепортов, одинаковый индекс для входного и выходного телепорта)
    /// </summary>
    /// <param name="index">индекс</param>
    /// <param name="type">тип</param>
    /// <returns></returns>
    public Edge GetEdgeBy(byte index, EdgeType type)
    {
      Edge result;
      _edgeByIndex.TryGetValue(new EdgeIndex
      {
        Type = type,
        Index = index
      }, out result);
      return result;
    }

    /// <summary>
    /// список пустых ячеек
    /// </summary>
    public ICollection<Tile> Empty { get { return _empty; } }

    /// <summary>
    /// добавить в список пустых ячеек
    /// </summary>
    /// <param name="tile"></param>
    public void AddToEmpty(Tile tile)
    {
      if (!_empty.Add(tile))
      {
        throw new ArgumentException("в списке уже содержится данная ячейка");
      }
    }

    /// <summary>
    /// удалить из списка пустых ячеек
    /// </summary>
    /// <param name="tile"></param>
    public void RemoveFromEmpty(Tile tile)
    {
      _empty.Remove(tile);
    }

    /// <summary>
    /// можно ли менять ячейки
    /// </summary>
    /// <param name="swap"></param>
    /// <returns></returns>
    public bool CanSwap(Swap swap)
    {
      if (_bounds.Contains(swap.First) && _bounds.Contains(swap.Second))
      {
        Tile firstTile;
        Tile secondTile;
        if (_tileMap.TryGetValue(swap.First, out firstTile) && _tileMap.TryGetValue(swap.Second, out secondTile))
        {
          if (firstTile.IsSwapable && secondTile.IsSwapable)
          {
            var edge = GetEdgeByTile(swap.First, swap.FirstDirection);
            if (edge.Type == EdgeType.Lock) return false;
            return _matchCombinations.FastMatch(swap, this);
          }
        }
      }
      return false;
    }

    /// <summary>
    /// можно ли менять ячейку с любой из соседних
    /// </summary>
    /// <param name="tile">координаты ячейки (в пространсве координат ячеек)</param>
    /// <returns></returns>
    public bool CanSwap(Point tile)
    {
      foreach (var offset in Point.Directions)
      {
        var second = tile + offset;
        if (_bounds.Contains(second))
        {
          if (CanSwap(new Swap(tile, second)))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// получить список всех доступных перемещений
    /// </summary>
    /// <param name="result">массив, в который запишеться результат, может быть null</param>
    /// <returns>указывает найдены ли доступные перемещения</returns>
    public bool AvailableSwaps(List<Swap> result = null)
    {
      foreach (var tile in Tiles)
      {
        if (tile.IsSwapable)
        {
          if (AvailableSwap(tile.Position, Direction.Right, result) && result == null) return true;
          if (AvailableSwap(tile.Position, Direction.Bottom, result) && result == null) return true;
        }
      }

      return result != null && result.Count != 0;
    }

    /// <summary>
    /// перемешать поле
    /// </summary>
    public bool Shuffle(ShuffleResult result = null)
    {
      var maxAttempts = 3;
      bool swaps;
      while ((swaps = AvailableSwaps()) == false && --maxAttempts >= 0)
      {
        var tiles = Tiles.Where(t => !t.IsEmpty && t.IsMovable && t.ItemType != ItemType.Artifact).ToList();
        while (tiles.Count > 0)
        {
          var tile = tiles[tiles.Count - 1];
          var next = tiles[_engineRandomState.GetNextRandom(tiles.Count - 1)];
          {
            tile.SetNextItem(next.Item);
            next.SetNextItem(tile.Item);

            tile.ApplyNextItem();
            next.ApplyNextItem();

            tiles.Remove(tile);
            tiles.Remove(next);

            if (result != null)
            {
              result.Shuffles.Add(new ShuffleResult.Swap
              {
                From = tile.Position,
                To = next.Position
              });
            }
          }
        }
      }
      if (swaps == false)
      {
        return false;
        //TODO: написать аглоритм перетасовки с возможными сопоставлениями
      }
      return true;
    }

    /**
     * ==================================================================
     */

    private void AddEdge(Edge edge)
    {
      _edgeMap[edge.Position] = edge;
      if (edge.Type == EdgeType.Input)
      {
        _inputs.Add(edge);
      }

      if (edge.IsTeleport)
      {
        _edgeByIndex.Add(new EdgeIndex { Type = edge.Type, Index = edge.Index }, edge);
      }
    }

    private bool AvailableSwap(Position tile, Direction direction, List<Swap> result)
    {
      var first = (Point)tile;
      var second = first + Point.Direction(direction);

      if (_bounds.Contains(second))
      {
        var swap = new Swap(first, second);
        if (CanSwap(swap))
        {
          if (result == null) return true;
          if (!result.Contains(swap))
          {
            result.Add(swap);
          }
        }
      }

      return false;
    }

    private struct EdgeIndex : IEquatable<EdgeIndex>
    {
      public byte Index;
      public EdgeType Type;

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        return obj is EdgeIndex && Equals((EdgeIndex)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          return (Index.GetHashCode() * 397) ^ (int)Type;
        }
      }

      public bool Equals(EdgeIndex other)
      {
        return Index == other.Index && Type == other.Type;
      }
    }
  }
}
