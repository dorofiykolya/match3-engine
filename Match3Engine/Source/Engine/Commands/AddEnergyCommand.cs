using System;
using Match3.Engine.InputActions;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Commands
{
  public class AddEnergyCommand : EngineCommand<AddEnergyInputAction>
  {
    protected override void Execute(AddEnergyInputAction action, Engine engine, IEngineStateInvalidator stateInvalidator)
    {
      if (action.Energy <= 0)
      {
        throw new InvalidOperationException("Energy не может быть 0 или отрицательным");
      }
      stateInvalidator.AddAdditionalEnergy(action.Energy);
      stateInvalidator.Invalidate();

      if (engine.Environment.IsGenerateOutputEvents())
      {
        engine.EnqueueByFactory<EnergyChangeEvent>(engine.Tick).Energy = stateInvalidator.Energy;
      }
    }
  }
}
