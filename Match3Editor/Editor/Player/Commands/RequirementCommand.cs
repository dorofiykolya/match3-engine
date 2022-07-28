using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Match3.Editor.Windows;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Editor.Player.Commands
{
  public class RequirementCommand : PlayerCommand<RequirementEvent>
  {
    protected override void Execute(RequirementEvent evt, PlayerContext context, LevelPlayer view)
    {
      context.Enqueue(Requirement(evt, context, view));
    }

    private IEnumerator Requirement(RequirementEvent evt, PlayerContext context, LevelPlayer view)
    {
      yield return null;
      List<RequirementPlayerItem> list;
      if (view.RequirementsPlaceholders.TryGetValue(evt.Type, out list))
      {
        if (evt.Type == LevelRequirementType.Stars)
        {
          foreach (var item in list)
          {
            item.SetValue(evt.Value);
          }
        }
        else
        {
          var item = list.FirstOrDefault(i => i.Id == evt.Id && i.Level == evt.Level);
          if (item != null)
          {
            item.SetValue(evt.Value);
          }
        }
      }
    }
  }
}
