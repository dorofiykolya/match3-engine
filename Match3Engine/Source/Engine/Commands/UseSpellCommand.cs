using Match3.Engine.InputActions;
using Match3.Engine.Levels;

namespace Match3.Engine.Commands
{
  public class UseSpellCommand : EngineCommand<UseSpellInputAction>
  {
    protected override void Execute(UseSpellInputAction action, Engine engine, IEngineStateInvalidator stateInvalidator)
    {
      stateInvalidator.UseSpell(new UseSpell
      {
        Id = action.Id,
        Level = action.Level,
        Positions = action.Positions,
        Type = action.Type,
        SpellType = action.SpellType
      });
    }
  }
}
