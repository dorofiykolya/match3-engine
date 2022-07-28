using System;
using System.Collections.Generic;

namespace Match3.Engine.Matches
{
  public class MatchCombinationsResult
  {
    private static readonly Comparer<Match> Comparer = Comparer<Match>.Default;
    private readonly List<Match> _combinations = new List<Match>();

    public bool HasMatches
    {
      get { return _combinations.Count != 0; }
    }

    public void AddMatch(Match combination)
    {
      _combinations.Add(combination);
    }

    public IList<Match> Combinations
    {
      get { return _combinations; }
    }

    public void Prune()
    {
      _combinations.Clear();
    }

    public void Merge(EngineStatePool pool)
    {
      _combinations.Sort(Comparer);
      var merged = pool.PopList<Match>();

      var combinations = pool.PopList<Match>();
      combinations.AddRange(_combinations);

      var map = pool.PopList<List<Match>>();

      while (combinations.Count != 0)
      {
        var current = combinations[0];
        var resultCombinations = pool.PopList<Match>();
        var currentCombinations = pool.PopList<Match>();
        currentCombinations.Add(current);
        for (var i = 1; i < combinations.Count; i++)
        {
          var next = combinations[i];
          if (!Equals(current, next))
          {
            resultCombinations.Add(combinations[i]);
          }
          else
          {
            currentCombinations.Add(combinations[i]);
          }
        }
        map.Add(currentCombinations);
        combinations.Clear();
        combinations.AddRange(resultCombinations);
        pool.PushList(resultCombinations);
      }

      foreach (var matches in map)
      {
        var match = matches[Math.Max(0, Ceil(matches.Count / 2f) - 1)];
        merged.Add(match);
      }

      //for (int i = 0; i < _combinations.Count; i++)
      //{
      //  if (!NeedRemove(i))
      //  {
      //    merged.Add(_combinations[i]);
      //  }
      //}

      _combinations.Clear();
      _combinations.AddRange(merged);
      _combinations.Sort(Comparer);
      _combinations.Reverse();

      foreach (var list in map)
      {
        pool.PushList(list);
      }
      pool.PushList(map);

      pool.PushList(combinations);
      pool.PushList(merged);
    }

    private int Ceil(float value)
    {
      int floor = (int)value;
      if (Math.Abs(value - floor) <= float.Epsilon)
      {
        return floor;
      }
      return floor + 1;
    }

    private bool Equals(Match match, Match next)
    {
      if (match.Combination.Count == next.Combination.Count)
      {
        for (int k = 0; k < match.Combination.Count; k++)
        {
          if (match.Combination[k] != next.Combination[k])
          {
            break;
          }
        }

        return true;
      }

      return false;
    }

    private bool NeedRemove(int startIndex)
    {
      var current = _combinations[startIndex];
      for (int j = startIndex + 1; j < _combinations.Count; j++)
      {
        var next = _combinations[j];
        if (current.Combination.Count == next.Combination.Count)
        {
          for (int k = 0; k < current.Combination.Count; k++)
          {
            if (current.Combination[k] != next.Combination[k])
            {
              break;
            }
          }
          return true;
        }
      }
      return false;
    }
  }
}
