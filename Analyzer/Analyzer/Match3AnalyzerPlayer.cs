using System;
using System.Collections.Generic;
using Match3.Engine;
using Match3.Engine.Levels;

namespace Match3.Analyzer
{
  public class Match3AnalyzerPlayer
  {
    private readonly Func<IEngine> _factory;

    public Match3AnalyzerPlayer(Func<IEngine> factory)
    {
      _factory = factory;
    }

    public AnalyzerChain FastForward(AnalyzerCancellation cancellation, IProgressHandler progressHandler = null)
    {
      var root = new AnalyzerChain(null, _factory, null, cancellation);
      if (!root.Engine.State.IsFinished)
      {
        root.Engine.FastForward(1);
        CreateChain(root, progressHandler, cancellation);
      }
      else if (progressHandler != null)
      {
        progressHandler.IncreaseVariant();
        if (root.Engine.State.Requirements.IsComplete)
        {
          progressHandler.IncreaseSuccess();
        }
        else
        {
          progressHandler.IncreaseFail();
        }
      }
      return root;
    }

    private void CreateChain(AnalyzerChain chain, IProgressHandler progressHandler, AnalyzerCancellation cancellation)
    {
      if (cancellation.IsCanceled) return;
      if (!chain.IsFinish)
      {
        var swaps = new List<Swap>();
        chain.Engine.State.AvailableSwaps(swaps);
        foreach (var swap in swaps)
        {
          if(cancellation.IsCanceled) return;

          CreateChain(new AnalyzerChain(chain, _factory, swap, cancellation), progressHandler, cancellation);

          if (progressHandler != null) progressHandler.IncreaseSwaps();
        }
      }
      else if (progressHandler != null)
      {
        progressHandler.IncreaseVariant();
        if (chain.Engine.State.Requirements.IsComplete)
        {
          progressHandler.IncreaseSuccess();
        }
        else
        {
          progressHandler.IncreaseFail();
        }
      }
    }
  }
}
