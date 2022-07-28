using System;
using System.Collections.Generic;
using Match3.Editor.Player.Commands;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;
using Match3.Player.Commands;

namespace Match3.Editor.Player
{
  public class PlayerCommandProcessor
  {
    private readonly Dictionary<Type, PlayerCommand> _commands = new Dictionary<Type, PlayerCommand>();

    public PlayerCommandProcessor()
    {
      Map<CreateEvent, CreateCommand>();
      Map<FallEvent, FallCommand>();
      Map<GenerateEvent, GenerateCommand>();
      Map<PreSwapEvent, PreSwapCommand>();
      Map<PostSwapEvent, PostSwapCommand>();
      Map<ActivateEvent, ActivateCommand>();
      Map<FinishEvent, FinishCommand>();
      Map<RequirementEvent, RequirementCommand>();
      Map<ScoreEvent, ScoreCommnad>();
      Map<IncreaseLevelEvent, IncreaseLevelCommand>();
      Map<ShuffleEvent, ShuffleCommand>();
      Map<ModifierEvent, ModifierCommand>();
      Map<SwapsChangedEvent, SwapsChangedCommand>();
      Map<SpellCombinationEvent, SpellCombinationCommand>();
      Map<ChangeItemEvent, ChangeItemCommand>();
      Map<ContinueEvent, ContinueCommand>();
      Map<AddSpellEvent, AddSpellCommand>();
      Map<SpellCountChangeEvent, SpellCountChangeCommand>();
      Map<ItemToOuputEvent, ItemToOutputCommand>();
      Map<UseSuiteSpellEvent, UseSuiteSpellCommand>();
      Map<StartOverEvent, StartOverCommand>();
      Map<StreakEvent, StreakCommand>();
      Map<EnergyChangeEvent, EnergyChangeCommand>();
      Map<BeginUseSpellEvent, BeginUseSpellCommand>();
      Map<EndUseSpellEvent, EndUseSpellCommand>();
      Map<RequirementsCompleteEvent, RequirementsCompleteCommand>();
      Map<UseSpellActionEvent, UseSpellActionCommand>();
    }

    private void Map<TEvent, TCommand>() where TCommand : PlayerCommand<TEvent>, new() where TEvent : OutputEvent
    {
      _commands[typeof(TEvent)] = new TCommand();
    }

    public void Execute(OutputEvent evt, PlayerContext context, LevelPlayer view)
    {
      PlayerCommand command = _commands[evt.GetType()];
      command.Execute(evt, context, view);
    }
  }
}
