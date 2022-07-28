using Match3.Engine.Descriptions.Items;

namespace Match3.Engine.Providers
{
  public interface IItemDescriptionProvider
  {
    ItemDescription Get(int id);
    int Count { get; }
    ItemDescription[] Collection { get; }
  }
}
