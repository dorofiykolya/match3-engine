using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Match3.Engine.Levels;
using Brush = System.Windows.Media.Brush;

namespace Match3.Editor.Utils
{
  public class EdgeTypeToColor
  {
    public static Brush ToColor(EdgeType type)
    {
      switch (type)
      {
        case EdgeType.None: return Brushes.White;
        case EdgeType.Input: return Brushes.DeepSkyBlue;
        case EdgeType.Output: return Brushes.Coral;
        case EdgeType.Lock: return Brushes.Red;
        case EdgeType.TeleportInput: return Brushes.LightGreen;
        case EdgeType.TeleportOutput: return Brushes.LightSeaGreen;
      }
      return Brushes.Transparent;
    }
  }
}
