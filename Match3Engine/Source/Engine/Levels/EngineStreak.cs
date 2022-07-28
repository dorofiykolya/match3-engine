using System.Collections.Generic;
using Match3.Engine.Matches;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Levels
{
  public class EngineStreak
  {
    public enum State
    {
      None,
      InProcess
    }

    private readonly EngineState _state;
    private int _matches;
    private State _streakState;

    public EngineStreak(EngineState state)
    {
      _state = state;
    }

    public void Begin()
    {
      if (_streakState != State.InProcess)
      {
        _streakState = State.InProcess;
        _matches = 0;
      }
    }

    public void End()
    {
      if (_streakState == State.InProcess)
      {
        if (_matches != 0)
        {
          if (_state.Environment.IsGenerateOutputEvents())
          {
            var evt = _state.Output.EnqueueByFactory<StreakEvent>(_state.Tick);
            evt.Matches = _matches;
          }

          _matches = 0;
        }

        _streakState = State.None;
      }
    }

    public void AddMatches(IList<Match> matches)
    {
      if (_streakState == State.InProcess)
      {
        _matches += matches.Count;
      }
    }
  }
}
