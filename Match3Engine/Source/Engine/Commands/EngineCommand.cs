using System;
using Match3.Engine.InputActions;

namespace Match3.Engine.Commands
{
  public abstract class EngineCommand
  {
    private readonly Type _actionType;

    public EngineCommand(Type inputActionType)
    {
      _actionType = inputActionType;
    }

    public Type ActionType
    {
      get { return _actionType; }
    }

    public abstract void Execute(InputAction action, Engine engine, IEngineStateInvalidator stateInvalidator);
  }

  public abstract class EngineCommand<T> : EngineCommand where T : InputAction
  {
    public EngineCommand() : base(typeof(T))
    {
    }

    public override void Execute(InputAction action, Engine engine, IEngineStateInvalidator stateInvalidator)
    {
      Execute((T) action, engine, stateInvalidator);
    }

    protected abstract void Execute(T action, Engine engine, IEngineStateInvalidator stateInvalidator);
  }
}
