using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Providers;
using Match3.Engine.Utils;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedItemDescriptionProvider : IItemDescriptionProvider
  {
    private readonly UnsignedIntDictionary<ItemDescription> _map;

    public SharedItemDescriptionProvider(ItemDescription[] items)
    {
      _map = new UnsignedIntDictionary<ItemDescription>(items.Length);
      foreach (var item in items)
      {
        _map[item.Id] = item;
      }
    }

    public int Count
    {
      get { return _map.Count; }
    }

    public ItemDescription[] Collection
    {
      get { return _map.Values.ToArray(); }
    }

    public ItemDescription Get(int id)
    {
      return _map[id];
    }
  }
}
