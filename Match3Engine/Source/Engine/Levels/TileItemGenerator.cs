using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Providers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Match3.Engine.Levels
{
  public class TileItemGenerator
  {
    private readonly IEngineNextRandom _randomEngineState;
    private readonly Dictionary<Point, Queue<Item>> _edgeMap;
    private readonly int[] _availableItems;

    public TileItemGenerator(ITileGridProvider tileGrid, LevelDescription levelDescription, IEngineNextRandom randomEngineState, IEngineProviders providers, EngineEnvironment environment)
    {
      // ReSharper disable NotResolvedInText
      if (levelDescription.AvailableItems == null) throw new ArgumentNullException(MethodBase.GetCurrentMethod().Name + ": levelDescription.AvailableItems == null");
      // ReSharper restore NotResolvedInText

      _randomEngineState = randomEngineState;
      _availableItems = levelDescription.AvailableItems;
      Array.Sort(_availableItems);
      _edgeMap = new Dictionary<Point, Queue<Item>>();

      foreach (var edgeDescription in levelDescription.Edges)
      {
        if (edgeDescription.Type == EdgeType.Input)
        {
          _edgeMap[edgeDescription.Position] = edgeDescription.Queue != null ? new Queue<Item>(edgeDescription.Queue) : new Queue<Item>();
        }
      }

      if (!environment.IsSkipDataCheck())
      {
        var itemsProvider = providers.ItemsProvider;
        foreach (var itemId in _availableItems)
        {
          if (itemsProvider.Get(itemId) == null)
          {
            throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": описания предмета не найдено, itemId:" +
                                        itemId);
          }
        }
      }
    }

    public Item GenerateItem(Edge edge)
    {
      Item item;
      Queue<Item> queue;
      if (_edgeMap.TryGetValue(edge.Position, out queue))
      {
        if (queue.Count != 0)
        {
          item = queue.Dequeue();
        }
        else
        {
          var index = _randomEngineState.GetNextRandom(_availableItems.Length);
          item = new Item { Id = _availableItems[index] };
        }
      }
      else
      {
        throw new ArgumentException(MethodBase.GetCurrentMethod().Name + ": попытка сгенерировать ячейку с несуществующей грани");
      }

      if (item == null) throw new NullReferenceException(MethodBase.GetCurrentMethod().Name + ": item == null");

      return item;
    }
  }
}
