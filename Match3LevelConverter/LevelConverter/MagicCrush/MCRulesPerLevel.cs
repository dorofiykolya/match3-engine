using System;
using Match3.LevelConverter.MagicCrush.RulesPerLevel;

namespace Match3.LevelConverter.MagicCrush
{
  [Serializable]
  public class MCRulesPerLevel
  {
    public string title;
    public string description;
    public int stepsPerLevel;
    public int useBoostWithId;
    public MCTasksPerLevel tasksPerLevel;
    public MCForbiddenSymbols forbiddenSymbols;
    public int[] scoresRequired;
  }
}
