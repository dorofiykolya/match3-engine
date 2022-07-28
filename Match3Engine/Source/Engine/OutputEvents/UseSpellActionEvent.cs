using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class UseSpellActionEvent : OutputEvent
  {
    public List<Point> ActivateTiles = new List<Point>(8);
    public List<Point> ChangeTiles = new List<Point>(8);
    public UseSpell UseSpell;

    public override void Reset()
    {
      ActivateTiles.Clear();
      ChangeTiles.Clear();
      UseSpell = null;
    }
  }
}
