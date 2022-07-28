using Match3.Engine.Descriptions.Modifiers;

namespace Match3.Engine.Providers
{
  public interface IModifierDescriptionProvider
  {
    ModifierDescription Get(int id);
  }
}
