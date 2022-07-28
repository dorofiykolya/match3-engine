using System;
using System.Collections.Generic;
using Match3.Engine.Levels;

namespace Match3.Engine.Matches
{
  public class Match : IComparable<Match>, IComparable
  {
    public IList<Point> Combination;
    public Item GenerateItem;
    public Point Pivot;
    public int Priority;

    public Match()
    {

    }

    public Match(Point pivot, IList<Point> combination)
    {
      Pivot = pivot;
      Combination = combination;
    }

    public Match(Point pivot, IList<Point> combination, Item generateItem)
    {
      Pivot = pivot;
      Combination = combination;
      GenerateItem = generateItem;
    }

    public Match(Point pivot, IList<Point> combination, Item generateItem, int priority)
    {
      Pivot = pivot;
      Combination = combination;
      GenerateItem = generateItem;
      Priority = priority;
    }

    public Match(Point pivot, IList<Point> combination, int priority)
    {
      Pivot = pivot;
      Combination = combination;
      Priority = priority;
    }

    public int CompareTo(Match other)
    {
      if (ReferenceEquals(this, other)) return 0;
      if (ReferenceEquals(null, other)) return 1;
      return Priority.CompareTo(other.Priority);
    }

    public int CompareTo(object obj)
    {
      var other = obj as Match;
      if (ReferenceEquals(null, other)) return 1;
      if (ReferenceEquals(this, other)) return 0;
      return Priority.CompareTo(other.Priority);
    }
  }
}
