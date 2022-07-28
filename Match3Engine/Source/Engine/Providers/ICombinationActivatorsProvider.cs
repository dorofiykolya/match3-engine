using Match3.Engine.Levels;

namespace Match3.Engine.Providers
{
  public interface ICombinationActivatorsProvider
  {
    SwapCombinationActivator GetSwapActivator(Item from, Item to);
    CombinationActivator GetActivator(Item item);
  }
}
