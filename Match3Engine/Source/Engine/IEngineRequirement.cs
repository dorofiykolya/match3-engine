using System;
namespace Match3.Engine
{
  public interface IEngineRequirement
  {
    bool IsComplete { get; }
  }
}
