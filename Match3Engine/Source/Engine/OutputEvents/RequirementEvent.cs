using Match3.Engine.Descriptions.Levels;

namespace Match3.Engine.OutputEvents
{
  public class RequirementEvent : OutputEvent
  {
    public LevelRequirementType Type;
    public int Value;
    public int Id;
    public int Level;

    public override void Reset()
    {
      Id = 0;
      Level = 0;
      Value = 0;
      Type = LevelRequirementType.None;
    }
  }
}
