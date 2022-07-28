using Match3.Engine.Levels;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.Utils
{
  public class EdgeTypeToPackIcon
  {
    public static PackIconKind ToPackIcon(EdgeType type)
    {
      switch (type)
      {
        case EdgeType.None: return PackIconKind.Delete;
        case EdgeType.Input: return PackIconKind.ArrowCollapseDown;
        case EdgeType.Output: return PackIconKind.ArrowCollapseUp;
        case EdgeType.Lock: return PackIconKind.Lock;
        case EdgeType.TeleportInput: return PackIconKind.DebugStepInto;
        case EdgeType.TeleportOutput: return PackIconKind.DebugStepOut;
      }
      return PackIconKind.ArrowTopLeft;
    }
  }
}
