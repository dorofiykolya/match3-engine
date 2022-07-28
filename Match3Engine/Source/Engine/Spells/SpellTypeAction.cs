using Match3.Engine.Levels;

namespace Match3.Engine.Spells
{
  public abstract class SpellTypeAction
  {
    public abstract void Execute(EngineState state, UseSpell useSpell);
  }
}
