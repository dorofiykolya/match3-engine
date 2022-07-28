using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  public class EngineRequirement : IEngineRequirement
  {
    private readonly EngineState _engineState;
    private readonly EngineEnvironment _environment;
    private readonly Dictionary<LevelRequirementType, List<Requirement>> _requirementsMap;
    private readonly int _maxStars;

    public EngineRequirement(EngineState engineState, LevelDescription level, IEngineProviders providers, EngineEnvironment environment)
    {
      _engineState = engineState;
      _environment = environment;
      _requirementsMap = new Dictionary<LevelRequirementType, List<Requirement>>();

      foreach (var requirement in level.Requirements)
      {
        List<Requirement> list;
        if (!_requirementsMap.TryGetValue(requirement.Type, out list))
        {
          _requirementsMap[requirement.Type] = list = new List<Requirement>();
        }

        if (requirement.Type == LevelRequirementType.Stars)
        {
          _maxStars++;
        }

        list.Add(new Requirement
        {
          Type = requirement.Type,
          Id = requirement.Id,
          Level = requirement.Level,
          Value = requirement.Value
        });
      }
    }

    public bool IsComplete
    {
      get
      {
        foreach (var pair in _requirementsMap)
        {
          if (pair.Key == LevelRequirementType.Stars)
          {
            if (pair.Value.Count >= _maxStars) return false;
          }
          else
          {
            if (pair.Value.Count != 0) return false;
          }
        }

        return true;
      }
    }

    public IEnumerable<Requirement> Remaining(LevelRequirementType type)
    {
      List<Requirement> list;
      if (_requirementsMap.TryGetValue(type, out list))
      {
        return list;
      }
      return null;
    }

    public int Remaining(int itemId, int itemLevel, LevelRequirementType type)
    {
      List<Requirement> list;
      if (_requirementsMap.TryGetValue(type, out list))
      {
        return list.Where(i => i.Id == itemId && i.Level == itemLevel).Sum(i => i.Value);
      }
      return 0;
    }

    public void Dispatch(int score, LevelRequirementType type)
    {
      List<Requirement> scoreList;
      if (_requirementsMap.TryGetValue(type, out scoreList))
      {
        Requirement item;
        while ((item = scoreList.Find(s => s.Value <= score)) != null)
        {
          scoreList.Remove(item);
          if (_environment.IsGenerateOutputEvents())
          {
            var evt = _engineState.Output.EnqueueByFactory<RequirementEvent>(_engineState.Tick);
            evt.Type = type;
            evt.Id = item.Id;
            evt.Level = item.Level;
            evt.Value = 0;
          }
        }
      }
    }

    public void Dispatch(int id, int level, LevelRequirementType type)
    {
      List<Requirement> list;
      if (_requirementsMap.TryGetValue(type, out list))
      {
        var result = list.FirstOrDefault(i => i.Id == id && (i.Level == 0 ||(i.Level != 0 && level != 0)));
        if (result != null)
        {
          if (type != LevelRequirementType.Modifier || level == 0)
          {
            result.Value--;
          }

          if (_environment.IsGenerateOutputEvents())
          {
            var evt = _engineState.Output.EnqueueByFactory<RequirementEvent>(_engineState.Tick);
            evt.Type = type;
            evt.Value = result.Value;
            evt.Id = result.Id;
            evt.Level = result.Level;
          }

          if (result.Value <= 0)
          {
            list.Remove(result);
          }
        }
      }
    }

    public class Requirement
    {
      public LevelRequirementType Type;
      public int Id;
      public int Level;
      public int Value;

      public Requirement Copy()
      {
        return (Requirement)MemberwiseClone();
      }
    }
  }
}

