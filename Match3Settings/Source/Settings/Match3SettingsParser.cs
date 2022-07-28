using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Descriptions.SpellCombinations;
using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Utils;

namespace Match3.Settings
{
  public class Match3SettingsParser
  {
    public static Match3Setting Parse(byte[] bytes)
    {
      var reader = new XmlTextReader(new MemoryStream(bytes));
      var document = XDocument.Load(reader);

      var result = new Match3Setting();

      var match3 = document.Element("match3");

      result.Descriptions = ParseDescriptions(match3);
      result.Items = ParseItems(match3);
      result.Modifiers = ParseModifiers(match3);
      result.Spells = ParseSpells(match3);
      result.SpellCombinations = ParseSpellCombinations(match3);

      return result;
    }

    private static SpellCombinationDescription[] ParseSpellCombinations(XElement document)
    {
      var list = new List<SpellCombinationDescription>();
      var elements = document.Element("spellcombinations");
      if (elements != null)
      {
        var spells = elements.Elements("spell");
        foreach (var spell in spells)
        {
          var id = int.Parse(spell.Attribute("id").Value);
          int level = 0;
          var levelAttribute = spell.Attribute("level");
          if (levelAttribute != null)
          {
            level = int.Parse(levelAttribute.Value);
          }
          var swaps = spell.Elements("swap");
          var swapList = new List<int>();
          foreach (var swap in swaps)
          {
            swapList.Add(int.Parse(swap.Attribute("id").Value));
          }
          list.Add(new SpellCombinationDescription
          {
            SpellId = id,
            SpellLevel = level,
            CombinationQueue = swapList.ToArray()
          });
        }
      }
      return list.ToArray();
    }

    private static SpellDescription[] ParseSpells(XElement document)
    {
      var list = new List<SpellDescription>();
      var elements = document.Element("spells");
      if (elements != null)
      {
        var spells = elements.Elements("spell");
        foreach (var spell in spells)
        {
          var description = spell.Attribute("description");
          list.Add(new SpellDescription
          {
            Id = int.Parse(spell.Attribute("id").Value),
            Description = description != null ? description.Value : "",
            Type = (SpellType)int.Parse(spell.Attribute("type").Value),
            UseType = (SpellUseType)int.Parse(spell.Attribute("useType").Value),
            UsePoints = int.Parse(spell.Attribute("usePoints").Value),
            UnlockLevel = int.Parse(spell.Attribute("unlockLevel").Value),
            Levels = ParseSpellLevels(spell)
          });
        }
      }
      return list.ToArray();
    }

    private static SpellLevelDescription[] ParseSpellLevels(XElement spell)
    {
      var list = new List<SpellLevelDescription>();
      var elements = spell.Element("levels");
      if (elements != null)
      {
        var levels = elements.Elements("level");
        foreach (var level in levels)
        {
          var minValue = level.Attribute("minValue");
          list.Add(new SpellLevelDescription
          {
            Value = int.Parse(level.Attribute("value").Value),
            Energy = int.Parse(level.Attribute("energy").Value),
            MinValue = minValue != null ? int.Parse(minValue.Value) : int.Parse(level.Attribute("value").Value)
          });
        }
      }

      return list.ToArray();
    }

    private static ModifierDescription[] ParseModifiers(XElement document)
    {
      var list = new List<ModifierDescription>();
      var elements = document.Element("modifiers");
      if (elements != null)
      {
        var modifiers = elements.Elements("modify");
        foreach (var modifier in modifiers)
        {
          var id = modifier.Attribute("id");
          var type = modifier.Attribute("type");
          var description = modifier.Attribute("description");
          var activationType = modifier.Attribute("activationType");
          //if (id != null && type != null)
          {
            list.Add(new ModifierDescription
            {
              Id = int.Parse(id.Value),
              Description = description != null ? description.Value : "",
              Type = (ModifierType)int.Parse(type.Value),
              ActivationType = activationType != null ? (ModifierActivatorType)int.Parse(activationType.Value) : ModifierActivatorType.Current
            });
          }
        }
      }
      return list.ToArray();
    }

    private static ItemDescription[] ParseItems(XElement document)
    {
      var list = new List<ItemDescription>();
      var items = document.Element("items");
      if (items != null)
      {
        var elements = items.Elements("item");
        foreach (var item in elements)
        {
          var id = item.Attribute("id");
          var description = item.Attribute("description");
          var type = item.Attribute("type");
          //if (id != null && type != null)
          {
            list.Add(new ItemDescription
            {
              Id = int.Parse(id.Value),
              Type = (ItemType)int.Parse(type.Value),
              Description = description != null ? description.Value : "",
              Levels = ParseItemLevel(item)
            });
          }
        }
      }
      return list.ToArray();
    }

    private static ItemLevelDescription[] ParseItemLevel(XElement item)
    {
      var list = new List<ItemLevelDescription>();
      var elements = item.Element("levels");
      if (elements != null)
      {
        var levels = elements.Elements("level");
        foreach (var level in levels)
        {
          var score = level.Attribute("score");
          var description = level.Attribute("description");
          //if (score != null)
          {
            list.Add(new ItemLevelDescription
            {
              Score = int.Parse(score.Value),
              Description = description != null ? description.Value : "",
              Activator = new ItemActivator
              {
                Offsets = PatternParser.Parse(level.Value, true)
              }
            });
          }
        }
      }
      return list.ToArray();
    }

    private static SettingDescription[] ParseDescriptions(XElement document)
    {
      var list = new List<SettingDescription>();
      var descriptions = document.Element("descriptions");
      if (descriptions != null)
      {
        var elements = descriptions.Elements("description");
        foreach (var element in elements)
        {
          var id = element.Attribute("id");
          //if (id != null)
          {
            list.Add(new SettingDescription
            {
              Id = id.Value,
              Value = element.Value
            });
          }
        }
      }
      return list.ToArray();
    }
  }
}
