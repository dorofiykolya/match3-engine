using System.Windows.Media;
using Match3.Engine.Levels;

namespace Match3.Editor.Utils
{
  public class ItemToColor
  {
    public static Brush ToColor(int itemId)
    {
      switch (itemId)
      {
        case ItemId.Red:
          return Brushes.Red;
        case ItemId.Blue:
          return Brushes.Blue;
        case ItemId.White:
          return Brushes.White;
        case ItemId.Yellow:
          return Brushes.Yellow;
        case ItemId.Violet:
          return Brushes.Violet;
        case ItemId.Green:
          return Brushes.Green;
        case ItemId.Universal:
          return Brushes.Chartreuse;

        case ItemId.Artifact:
          return Brushes.Cyan;
      }
      return Brushes.White;
    }
  }
}
