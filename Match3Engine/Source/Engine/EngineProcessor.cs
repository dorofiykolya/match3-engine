using System;
using System.Reflection;
using Match3.Engine.InputActions;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  public class EngineProcessor
  {
    private readonly ICommandsProvider _provider;

    public EngineProcessor(ICommandsProvider provider)
    {
      _provider = provider;
    }

    public void Execute(InputAction action, Engine engine, EngineState state)
    {
      var command = _provider.GetCommand(action.GetType());

      if (engine.Configuration.Environment.IsDebug())
      {
        if (command == null)
        {
          throw new InvalidOperationException(string.Format(MethodBase.GetCurrentMethod().Name + ": комманда не найдена: {0}", action.GetType()));
        }
      }

      if (command != null)
      {
        command.Execute(action, engine, state);
      }
    }
  }
}
