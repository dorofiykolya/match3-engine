using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.Matches
{
  public class Match4HorizontalCombination : MatchCombination
  {
    public Match4HorizontalCombination(int priority) : base(priority,
      new Dictionary<int, Item>
      {
        { ItemId.White,  new Item{Id = ItemId.White,  Level = LevelId.L1} },
        { ItemId.Blue,   new Item{Id = ItemId.Blue,   Level = LevelId.L1} },
        { ItemId.Green,  new Item{Id = ItemId.Green,  Level = LevelId.L1} },
        { ItemId.Red,    new Item{Id = ItemId.Red,    Level = LevelId.L1} },
        { ItemId.Violet, new Item{Id = ItemId.Violet, Level = LevelId.L1} },
        { ItemId.Yellow, new Item{Id = ItemId.Yellow, Level = LevelId.L1} },
      })
    {
      //4x1
      AddPattern(@"[#|#|#|#]");
    }
  }
}
