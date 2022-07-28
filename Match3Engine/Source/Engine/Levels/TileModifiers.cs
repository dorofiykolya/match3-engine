using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Providers;
using System;
using System.Collections.Generic;

namespace Match3.Engine.Levels
{
  public class TileModifiers
  {
    private readonly Modifier[] _modifiers;
    private readonly IModifierDescriptionProvider _modifierDescriptionProvider;

    public TileModifiers(Modifier[] modifiers, IEngineProviders providers)
    {
      _modifiers = modifiers;
      _modifierDescriptionProvider = providers.ModifierDescriptionProvider;
    }

    public IEnumerable<Modifier> Modifiers
    {
      get { return _modifiers; }
    }

    public bool CanSwap
    {
      get
      {
        if (_modifiers.Length == 0) return true;
        foreach (var modifier in _modifiers)
        {
          var type = _modifierDescriptionProvider.Get(modifier.Id).Type;
          if (type == ModifierType.Armor || type == ModifierType.Box)
          {
            if (modifier.Level > 0) return false;
          }
        }
        return true;
      }
    }

    public bool CanMove
    {
      get
      {
        if (_modifiers.Length == 0) return true;
        foreach (var modifier in _modifiers)
        {
          var type = _modifierDescriptionProvider.Get(modifier.Id).Type;
          if (type == ModifierType.Armor || type == ModifierType.Box)
          {
            if (modifier.Level > 0) return false;
          }
        }
        return true;
      }
    }

    public bool CanReceiveItem
    {
      get
      {
        if (_modifiers.Length == 0) return true;
        foreach (var modifier in _modifiers)
        {
          var type = _modifierDescriptionProvider.Get(modifier.Id).Type;
          if (type == ModifierType.Box)
          {
            if (modifier.Level > 0) return false;
          }
        }
        return true;
      }
    }

    public void ApplyCollect(ModifierActivatorType activatorType, List<Modifier> modifies = null)
    {
      if (_modifiers.Length != 0)
      {
        foreach (var modifier in _modifiers)
        {
          var modifierActivationType = _modifierDescriptionProvider.Get(modifier.Id).ActivationType;
          var currentValue = modifierActivationType & activatorType;
          if (currentValue != 0)
          {
            if (modifier.Level > 0)
            {
              modifier.Level -= 1;
              if (modifies != null)
              {
                modifies.Add(modifier);
              }
            }
          }
        }
      }
    }
  }
}
