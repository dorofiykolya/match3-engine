using System;
using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Descriptions.SpellCombinations;
using Match3.Engine.Descriptions.Spells;

namespace Match3.Settings
{
  [Serializable]
  public class Match3Setting
  {
    public SettingDescription[] Descriptions;
    public ItemDescription[] Items;
    public ModifierDescription[] Modifiers;
    public SpellDescription[] Spells;
    public SpellCombinationDescription[] SpellCombinations;

    public string GetDescription(string id)
    {
      var description = Descriptions.FirstOrDefault(d => d.Id == id);
      if (description != null) return description.Value;
      return id != null ? id : string.Empty;
    }

    public ItemDescription GetItemDescription(int itemId)
    {
      var item = Items.FirstOrDefault(i => i.Id == itemId);
      return item ?? null;
    }

    public ModifierType GetModifierType(int modifierId)
    {
      var modifier = Modifiers.FirstOrDefault(m => m.Id == modifierId);
      if (modifier != null)
      {
        return modifier.Type;
      }
      return ModifierType.None;
    }
  }
}
