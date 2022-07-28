using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.Matches
{
  public class Match5Combination : MatchCombination
  {
    public Match5Combination(int priority) : base(priority,
      new Dictionary<int, Item>
      {
        { ItemId.White,  new Item{Id = ItemId.Universal, Level = LevelId.L0} },
        { ItemId.Blue,   new Item{Id = ItemId.Universal, Level = LevelId.L0} },
        { ItemId.Green,  new Item{Id = ItemId.Universal, Level = LevelId.L0} },
        { ItemId.Red,    new Item{Id = ItemId.Universal, Level = LevelId.L0} },
        { ItemId.Violet, new Item{Id = ItemId.Universal, Level = LevelId.L0} },
        { ItemId.Yellow, new Item{Id = ItemId.Universal, Level = LevelId.L0} },
      })
    {
      AddPattern(@"[#|#|#|#|#]");

      AddPattern(@"[#]" +
                  "[#]" +
                  "[#]" +
                  "[#]" +
                  "[#]");
    }
  }
}
