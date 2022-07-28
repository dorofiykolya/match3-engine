using System;
using Match3.Engine.InputActions;

namespace Match3.Engine.Commands
{
  public class AddSpellCommand : EngineCommand<AddSpellInputAction>
  {
    protected override void Execute(AddSpellInputAction action, Engine engine, IEngineStateInvalidator stateInvalidator)
    {
      if (action.Count <= 0)
      {
        throw new InvalidOperationException("Count не может быть 0 или отрицательным");
      }

      var spellProvider = engine.Configuration.Providers.SpellDescriptionProvider;
      var spell = spellProvider.Get(action.Id);
      if (spell == null)
      {
        throw new InvalidOperationException(string.Format("Spell с идентификатором Id:{0} не существует", action.Id));
      }

      var spellLevel = spell.GetLevel(action.Level);
      if (spellLevel == null)
      {
        throw new InvalidOperationException(string.Format("Spell Id:{0} с уровнем Level:{1} не существует", action.Id, action.Level));
      }

      stateInvalidator.AddSpell(action.Id, action.Level, action.Count);
    }
  }
}
