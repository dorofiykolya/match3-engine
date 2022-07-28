using System.Collections.Generic;
using Match3.Engine.Levels;
using Match3.Engine.Utils;

namespace Match3.Engine.Matches
{
  public class MatchPattern
  {
    private readonly List<Point[]> _offsets = new List<Point[]>();

    public void AddPattern(string pattern)
    {
      _offsets.AddRange(MatchPatternParser.Parse(pattern));
    }

    public IEnumerable<Point[]> Offsets
    {
      get { return _offsets; }
    }
  }
}
