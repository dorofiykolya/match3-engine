using System;

namespace Match3.LevelConverter.MagicCrush
{
  [Serializable]
  public class MCTasksPerLevel
  {
    public int green;
    public int yellow;
    public int white;
    public int blue;
    public int violet;
    public int red;

    public int level2_green;
    public int level2_yellow;
    public int level2_white;
    public int level2_blue;
    public int level2_violet;
    public int level2_red;

    public int level3;

    public int unique;

    public int locked_front;

    public int locked_back;

    public int useBoost;
  }
}
