using System;
using System.Collections.Generic;
using Match3.Engine.Levels;
using Match3.Engine.Matches;

namespace Match3.Engine
{
  public class EngineStatePool
  {
    private readonly Dictionary<Type, object> _listPool = new Dictionary<Type, object>();
    private readonly MatchMergeData _mergeData = new MatchMergeData();
    private readonly ModifierActivateData _modifierActivateData = new ModifierActivateData();
    private readonly MatchCombinationsResult _combinationsResult = new MatchCombinationsResult();

    public MatchCombinationsResult GetMatchCombinationsResult()
    {
      _combinationsResult.Prune();
      return _combinationsResult;
    }

    public MatchMergeData GetMergeData()
    {
      _mergeData.Clear();
      return _mergeData;
    }

    public ModifierActivateData GetModifierActivateData()
    {
      _modifierActivateData.Clear();
      return _modifierActivateData;
    }

    public List<T> PopList<T>()
    {
      object stack;
      if (!_listPool.TryGetValue(typeof(T), out stack))
      {
        _listPool[typeof(T)] = stack = new Stack<List<T>>();
      }
      var typed = (Stack<List<T>>)stack;
      return typed.Count == 0 ? new List<T>() : typed.Pop();
    }

    public void PushList<T>(List<T> list)
    {
      object stack;
      if (!_listPool.TryGetValue(typeof(T), out stack))
      {
        _listPool[typeof(T)] = stack = new Stack<List<T>>();
      }
      var typed = (Stack<List<T>>)stack;
      list.Clear();
      typed.Push(list);
    }
  }
}
