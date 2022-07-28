using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class EndUseSpellEvent : OutputEvent
  {
    public UseSpell UseSpell;

    public override void Reset()
    {
      UseSpell = null;
      base.Reset();
    }
  }
}
