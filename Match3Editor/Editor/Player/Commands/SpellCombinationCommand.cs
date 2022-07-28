using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using Match3.Editor.Windows;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class SpellCombinationCommand : PlayerCommand<SpellCombinationEvent>
  {
    protected override void Execute(SpellCombinationEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Do(evt, context, view));
    }

    private IEnumerator Do(SpellCombinationEvent evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;

      view.AvailableSpells.Children.Clear();
      view.PossibleSpells.Children.Clear();

      var i = 0;
      foreach (var description in evt.Available)
      {
        i++;
        var label = new Label();
        label.Background = i % 2 == 0? Brushes.DarkGray : Brushes.LightGray;
        label.Content = description.SpellId + ":" + description.SpellLevel;
        view.AvailableSpells.Children.Add(label);
      }

      foreach (var description in evt.Possible)
      {
        i++;
        var label = new Label();
        label.Background = i % 2 == 0 ? Brushes.DarkGray : Brushes.LightGray;
        label.Content = description.SpellId + ":" + description.SpellLevel;
        view.PossibleSpells.Children.Add(label);
      }
    }
  }
}
