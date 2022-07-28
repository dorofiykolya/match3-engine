using System;
using Match3.Engine.Commands;
using Match3.Engine.Providers;
using System.Collections.Generic;

namespace Match3.Engine.Shareds.Providers
{
  public class SharedCommandsProvider : ICommandsProvider
  {
    private readonly Dictionary<Type, EngineCommand> _commandMap = new Dictionary<Type, EngineCommand>();

    public SharedCommandsProvider()
    {
      Map(new SwapCommand());
      Map(new UseSpellCommand());
      Map(new AddSwapCommand());
      Map(new AddSpellCommand());
      Map(new AddEnergyCommand());
    }

    public void Map<T>(T value) where T : EngineCommand
    {
      _commandMap[value.ActionType] = value;
    }

    public EngineCommand GetCommand(Type actionType)
    {
      return _commandMap[actionType];
    }
  }
}
