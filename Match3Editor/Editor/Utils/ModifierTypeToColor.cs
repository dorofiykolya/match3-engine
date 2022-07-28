using System.Windows.Media;
using Match3.Engine.Descriptions.Modifiers;

namespace Match3.Editor.Utils
{
  public class ModifierTypeToColor
  {
    public static Brush ToColor(ModifierType type)
    {
      switch (type)
      {
        case ModifierType.Armor:
          return Brushes.Cyan;
        case ModifierType.Substrate:
          return Brushes.Magenta;
        case ModifierType.Box:
          return Brushes.Coral;
      }
      return Brushes.Black;
    }
  }
}
