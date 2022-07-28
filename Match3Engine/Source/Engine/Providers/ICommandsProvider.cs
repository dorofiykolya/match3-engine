using System;
using Match3.Engine.Commands;

namespace Match3.Engine.Providers
{
  public interface ICommandsProvider
  {
    EngineCommand GetCommand(Type actionType);
  }
}
