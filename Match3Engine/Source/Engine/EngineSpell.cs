using System;
using System.Collections.Generic;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;
using Match3.Engine.Providers;

namespace Match3.Engine
{
  public class EngineSpell
  {
    private readonly EngineState _state;
    private readonly IEngineProviders _providers;
    private Dictionary<int, List<Spell>> _map;

    public EngineSpell(EngineState state, Spell[] spells, IEngineProviders providers)
    {
      _state = state;
      _providers = providers;
      _map = new Dictionary<int, List<Spell>>();
      _map = new Dictionary<int, List<Spell>>();
      if (spells != null)
      {
        foreach (var spell in spells)
        {
          AddInternal(spell.Id, spell.Level, spell.Count, true);
        }
      }
    }

    public Spell[] Spells
    {
      get
      {
        var list = _state.Pool.PopList<Spell>();
        foreach (var pair in _map)
        {
          foreach (var spell in pair.Value)
          {
            list.Add(spell);
          }
        }

        var result = list.ToArray();
        _state.Pool.PushList(list);
        return result;
      }
    }

    public void GetSpells(List<Spell> spells)
    {
      foreach (var pair in _map)
      {
        foreach (var spell in pair.Value)
        {
          spells.Add(spell);
        }
      }
    }

    public void UseSpell(int id, int level)
    {
      if (!Contains(id, level) && !_state.Environment.IsAvailableAllSpells())
      {
        throw new InvalidOperationException(string.Format("Спел не доступен Id:{0}, Level:{1}", id, level));
      }

      List<Spell> levels;
      if (_map.TryGetValue(id, out levels))
      {
        foreach (var spell in levels)
        {
          if (spell.Id == id && spell.Level == level)
          {
            spell.Count--;

            if (_state.Environment.IsGenerateOutputEvents())
            {
              var evt = _state.Output.EnqueueByFactory<UseSuiteSpellEvent>(_state.Tick);
              evt.Id = id;
              evt.Level = level;


              var evtChange = _state.Output.EnqueueByFactory<SpellCountChangeEvent>(_state.Tick);
              evtChange.Id = id;
              evtChange.Level = level;
              evtChange.Count = spell.Count;
            }

            break;
          }
        }
      }
    }

    public void AddSpell(int id, int level, int count)
    {
      var spell = AddInternal(id, level, count, false);

      if (_state.Environment.IsGenerateOutputEvents())
      {
        var evt = _state.Output.EnqueueByFactory<AddSpellEvent>(_state.Tick);
        evt.Id = id;
        evt.Level = level;
        evt.Count = count;

        var evtChange = _state.Output.EnqueueByFactory<SpellCountChangeEvent>(_state.Tick);
        evtChange.Id = spell.Id;
        evtChange.Level = spell.Level;
        evtChange.Count = spell.Count;
      }
    }

    private Spell AddInternal(int id, int level, int count, bool byConstructor)
    {
      if (count <= 0 && !byConstructor)
      {
        throw new InvalidOperationException("Count не может быть 0 или отрицательным");
      }

      var spellProvider = _providers.SpellDescriptionProvider;
      var spell = spellProvider.Get(id);
      if (spell == null)
      {
        throw new InvalidOperationException(string.Format("Spell с идентификатором Id:{0} не существует", id));
      }

      var spellLevel = spell.GetLevel(level);
      if (spellLevel == null)
      {
        throw new InvalidOperationException(string.Format("Spell Id:{0} с уровнем Level:{1} не существует", id, level));
      }

      List<Spell> levels;
      if (!_map.TryGetValue(id, out levels))
      {
        _map[id] = levels = new List<Spell>();
      }

      Spell result;
      var finded = levels.Find(s => s.Id == id && s.Level == level);
      if (finded != null)
      {
        finded.Count += count;
        result = finded;
      }
      else
      {
        result = new Spell
        {
          Id = id,
          Level = level,
          Count = count
        };
        levels.Add(result);
      }

      return result;
    }

    public bool Contains(int id, int level)
    {
      List<Spell> spells;
      if (_map.TryGetValue(id, out spells))
      {
        foreach (var spell in spells)
        {
          if (spell.Id == id && spell.Level == level)
          {
            return spell.Count > 0;
          }
        }
      }

      return false;
    }
  }
}
