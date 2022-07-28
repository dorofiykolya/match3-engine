using Match3.Engine.Descriptions.SpellCombinations;

namespace Match3.Engine.OutputEvents
{
  public class SpellCombinationEvent : OutputEvent
  {
    public SpellCombinationDescription[] Available;
    public SpellCombinationDescription[] Possible;
    public int[] ItemsQueue;
  }
}
