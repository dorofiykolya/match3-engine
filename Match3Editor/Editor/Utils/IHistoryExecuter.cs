using System;

namespace Match3.Editor.Utils
{
  public interface IHistoryExecuter
  {
    void Execute(Action redo, Action undo, string description);
  }
}
