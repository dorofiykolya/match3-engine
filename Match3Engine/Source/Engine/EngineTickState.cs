using System;
namespace Match3.Engine
{
  public enum EngineTickState
  {
    None,
    PreTick,
    Command,
    PostTick,
    FinalizeTick
  }
}
