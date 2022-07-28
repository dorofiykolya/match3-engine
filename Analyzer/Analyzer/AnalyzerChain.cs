using System;
using System.Collections.Generic;
using Match3.Engine;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;

namespace Match3.Analyzer
{
  public class AnalyzerChain
  {
    private readonly Func<IEngine> _factory;
    private readonly IEngine _engine;
    private readonly Swap _swap;
    private readonly AnalyzerCancellation _cancellation;
    private readonly int _tick = 1;
    private readonly List<AnalyzerChain> _children;
    private readonly AnalyzerChain _parent;

    public AnalyzerChain(AnalyzerChain parent, Func<IEngine> factory, Swap swap, AnalyzerCancellation cancellation)
    {
      _children = new List<AnalyzerChain>();
      _swap = swap;
      _cancellation = cancellation;
      _factory = factory;
      _engine = _factory();
      _parent = parent;

      if (parent != null)
      {
        _tick = parent._tick + 1;
        _parent._children.Add(this);
      }

      FastForward();
    }

    public int Depth
    {
      get
      {
        var current = _parent;
        var depth = 0;
        while (current != null)
        {
          ++depth;
          current = current._parent;
        }
        return depth;
      }
    }
    public AnalyzerChain Root { get { return _parent == null ? this : _parent.Root; } }
    public bool IsRoot { get { return _parent == null; } }
    public IEngine Engine { get { return _engine; } }
    public bool IsFinish { get { return _engine.State.IsFinished; } }

    public List<AnalyzerChain> GetLastChains(List<AnalyzerChain> result = null)
    {
      if (result == null) result = new List<AnalyzerChain>();
      if (_children.Count == 0)
      {
        result.Add(this);
      }
      else
      {
        foreach (var child in _children)
        {
          child.GetLastChains(result);
        }
      }
      return result;
    }

    public LinkedList<AnalyzerChain> ToRoot()
    {
      var result = new LinkedList<AnalyzerChain>();
      var current = this;
      while (current != null)
      {
        result.AddFirst(current);
        current = current._parent;
      }
      return result;
    }

    private void FastForward()
    {
      if (_cancellation.IsCanceled) return;
      if (_swap != null)
      {
        var list = ToRoot();
        foreach (var chain in list)
        {
          if (_cancellation.IsCanceled) return;
          if (!chain.IsRoot)
          {
            _engine.AddAction(new SwapInputAction
            {
              Swap = chain._swap,
              Tick = chain._tick
            });
          }
        }

        if (_cancellation.IsCanceled) return;
        _engine.FastForward(_tick);
      }
    }
  }
}
