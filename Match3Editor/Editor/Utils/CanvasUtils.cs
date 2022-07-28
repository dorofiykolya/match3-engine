using System.Windows;
using System.Windows.Controls;

namespace Match3.Editor.Utils
{
  public class CanvasUtils
  {
    public static void SetPosition(UIElement element, Point position)
    {
      Canvas.SetLeft(element, position.X);
      Canvas.SetTop(element, position.Y);
    }
  }
}
