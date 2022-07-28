using System;
using System.Collections.Generic;

namespace Match3.Engine.InputActions
{
  public class InputAction : IComparable
  {
    private static readonly Comparer<int> Comparer = Comparer<int>.Default;

    public int Tick;

    public int CompareTo(object obj)
    {
      var other = obj as InputAction;
      if (other == null) throw new ArgumentException("обьект не является производным от: " + typeof(InputAction));
      return Comparer.Compare(Tick, other.Tick);
    }
  }
}
