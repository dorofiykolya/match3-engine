using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Match3.Editor.Player;
using Match3.Editor.Utils;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Descriptions.Spells;
using Match3.Engine.Providers;
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;

namespace Match3.Editor.LevelEditor
{
  /// <summary>
  /// Логика взаимодействия для LevelEditorRequirements.xaml
  /// </summary>
  public partial class LevelEditorRequirements : UserControl, INotifyPropertyChanged
  {
    private readonly Dictionary<int, Dictionary<int, LevelEditorRequirementItem>> _itemsMap = new Dictionary<int, Dictionary<int, LevelEditorRequirementItem>>();
    private readonly Dictionary<int, Dictionary<int, LevelEditorRequirementArtifact>> _artifactsMap = new Dictionary<int, Dictionary<int, LevelEditorRequirementArtifact>>();
    private readonly Dictionary<int, LevelEditorRequirementModifier> _modifiersMap = new Dictionary<int, LevelEditorRequirementModifier>();
    private readonly Dictionary<int, LevelEditorRequirementSpell> _spellMap = new Dictionary<int, LevelEditorRequirementSpell>();

    private int _swap;
    private int[] _availableItems;
    private IHistoryExecuter _historyContext;
    private ItemDescription[] _artifacts;
    private ItemDescription[] _cells;
    private ModifierDescription[] _modifiers;
    private Dictionary<int, int> _cellLevels;
    private int[] _modifierItems;
    private int[] _artifactItems;
    private int[] _artifactLevels;
    private int _addNewDialogTab;
    private ItemRequirementType _addNewItemRequirementType;
    private int _addNewItemId;
    private int _addNewItemLevel;
    private int _addNewArtifactId;
    private int _addNewArtifactLevel;
    private int _addNewModifierId;
    private ModifierType _addNewModifierType;
    private int _addNewModifierCount;
    private LevelEditorTileGridControl _tileGrid;
    private int[] _spellItems;
    private int _addNewSpellId;
    private int _addNewSpellCount;

    public int Swaps
    {
      get { return _swap; }
      set
      {
        if (_swap != value)
        {
          var last = _swap;
          _historyContext.Execute(() =>
          {
            _swap = value;
            OnPropertyChanged(nameof(Swaps));
            OnPropertyChanged(nameof(SwapIsValid));
          }, () =>
          {
            _swap = last;
            OnPropertyChanged(nameof(Swaps));
            OnPropertyChanged(nameof(SwapIsValid));
          }, $"change swap: '{_swap}'->'{value}'");
        }
      }
    }

    public EnumValueConverter<ItemRequirementType> ItemRequirementTypeEnum { get; set; } = new EnumValueConverter<ItemRequirementType>();
    public EnumValueConverter<ModifierType> ModifiersTypeEnum { get; set; } = new EnumValueConverter<ModifierType>();
    public EnumValueConverter<ItemType> ItemTypeEnum { get; set; } = new EnumValueConverter<ItemType>();

    public int[] AvailableItems { get { return _availableItems; } }
    public int[] ModifierItems { get { return _modifierItems.Where(i => !_modifiersMap.ContainsKey(i)).ToArray(); } }
    public int[] SpellItems { get { return _spellItems; } }

    public int[] CellLevels
    {
      get
      {
        if (AvailableItems.Contains(AddNewItemId))
        {
          return Enumerable.Range(0, _cellLevels[AddNewItemId] + 1).ToArray();
        }

        return new int[0];
      }
    }

    public int[] ArtifactItems { get { return _artifactItems; } }
    public int[] ArtifactLevels { get { return _artifactLevels; } }
    public MaterialDesignThemes.Wpf.PackIconKind CheckIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Check; } }
    public MaterialDesignThemes.Wpf.PackIconKind AlertIcon { get { return MaterialDesignThemes.Wpf.PackIconKind.Alert; } }
    public bool SwapIsValid { get { return _swap.ToString() == SwapInput.Text; } }

    public LevelEditorRequirements()
    {
      Initialize();
      InitializeComponent();
    }

    public void Initialize()
    {
      _artifacts = AppSettings.Setting.Items.Where(i => i.Type == ItemType.Artifact).ToArray();
      _spellItems = AppSettings.Setting.Spells.Where(s => s.UseType == SpellUseType.Rune).Select(s => s.Id).OrderBy(s => s).ToArray();
      _cells = AppSettings.Setting.Items.Where(i => i.Type == ItemType.Cell || i.Type == ItemType.UniversalSwapCell).OrderBy(i => i.Id).ToArray();
      _modifiers = AppSettings.Setting.Modifiers;
      _cellLevels = new Dictionary<int, int>();
      foreach (var cell in _cells)
      {
        _cellLevels[cell.Id] = cell.Levels.Length - 1;
      }

      _modifierItems = _modifiers.Select(c => c.Id).Distinct().OrderBy(i => i).ToArray();
      _artifactItems = _artifacts.Select(c => c.Id).Distinct().OrderBy(i => i).ToArray();
      _artifactLevels = _artifacts.Select(c => Enumerable.Range(0, c.Levels.Length)).SelectMany(s => s).Distinct().OrderBy(i => i).ToArray(); ;
    }

    public LevelRequirementDescription[] Requirements
    {
      get
      {
        var list = new List<LevelRequirementDescription>();
        foreach (var pair in _itemsMap)
        {
          foreach (var p in pair.Value)
          {
            if (p.Value.IsEnabled && p.Value.ItemChecked)
            {
              list.Add(new LevelRequirementDescription
              {
                Type = LevelRequirementType.CollectItem,
                Id = pair.Key,
                Level = p.Key,
                Value = p.Value.Value
              });
            }
          }
        }
        foreach (var pair in _artifactsMap)
        {
          foreach (var p in pair.Value)
          {
            if (p.Value.IsEnabled && p.Value.ItemChecked)
            {
              list.Add(new LevelRequirementDescription
              {
                Type = LevelRequirementType.CollectItem,
                Id = pair.Key,
                Level = p.Key,
                Value = p.Value.Value
              });
            }
          }
        }
        foreach (var modifier in _modifiersMap.Values)
        {
          if (modifier.IsEnabled && modifier.IsChecked)
          {
            if (modifier.Value > 0)
            {
              list.Add(new LevelRequirementDescription
              {

                Type = LevelRequirementType.Modifier,
                Id = modifier.Id,
                Value = modifier.Value
              });
            }
          }
        }
        foreach (var spell in _spellMap.Values)
        {
          if (spell.IsEnabled && spell.ItemChecked)
          {
            if (spell.Value > 0)
            {
              list.Add(new LevelRequirementDescription
              {
                Type = LevelRequirementType.UseSpell,
                Id = spell.ItemId,
                Level = 0,
                Value = spell.Value
              });
            }
          }
        }

        int star1;
        int star2;
        int star3;
        int.TryParse(Star1.Text, out star1);
        int.TryParse(Star2.Text, out star2);
        int.TryParse(Star3.Text, out star3);


        list.Add(new LevelRequirementDescription
        {
          Id = 0,
          Level = 1,
          Type = LevelRequirementType.Stars,
          Value = star1
        });
        list.Add(new LevelRequirementDescription
        {
          Id = 0,
          Level = 2,
          Type = LevelRequirementType.Stars,
          Value = star2
        });
        list.Add(new LevelRequirementDescription
        {
          Id = 0,
          Level = 3,
          Type = LevelRequirementType.Stars,
          Value = star3
        });
        return list.ToArray();
      }
    }

    [Bindable(true)]
    public IHistoryExecuter HistoryContext
    {
      get { return _historyContext; }
      set { _historyContext = value; }
    }

    [Bindable(true)]
    public LevelEditorTileGridControl TileGrid
    {
      get { return _tileGrid; }
      set { _tileGrid = value; }
    }

    public int AddNewDialogTab
    {
      get { return _addNewDialogTab; }
      set
      {
        _addNewDialogTab = value;
        OnPropertyChanged(nameof(AddNewDialogTab));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
      }
    }

    public ItemRequirementType AddNewItemRequirementType
    {
      get { return _addNewItemRequirementType; }
      set
      {
        _addNewItemRequirementType = value;
        OnPropertyChanged(nameof(AddNewItemRequirementType));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
      }
    }

    public int AddNewItemId
    {
      get { return _addNewItemId; }
      set
      {
        _addNewItemId = value;
        OnPropertyChanged(nameof(AddNewItemId));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
        OnPropertyChanged(nameof(CellLevels));
      }
    }

    public int AddNewItemLevel
    {
      get { return _addNewItemLevel; }
      set
      {
        _addNewItemLevel = value;
        OnPropertyChanged(nameof(AddNewItemLevel));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
      }
    }

    public int AddNewArtifactId
    {
      get { return _addNewArtifactId; }
      set
      {
        _addNewArtifactId = value;
        OnPropertyChanged(nameof(AddNewArtifactId));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
      }
    }

    public int AddNewArtifactLevel
    {
      get { return _addNewArtifactLevel; }
      set
      {
        _addNewArtifactLevel = value;
        OnPropertyChanged(nameof(AddNewArtifactLevel));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
      }
    }

    public int AddNewModifierId
    {
      get { return _addNewModifierId; }
      set
      {
        _addNewModifierId = value;
        OnPropertyChanged(nameof(AddNewModifierId));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
        OnPropertyChanged(nameof(AddNewButtonCalculateIsValid));
      }
    }

    public int[] ModifierCounts
    {
      get { return Enumerable.Range(0, 100).ToArray(); }
    }

    public int AddNewModifierCount
    {
      get { return _addNewModifierCount; }
      set
      {
        _addNewModifierCount = value;
        OnPropertyChanged(nameof(AddNewModifierCount));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
        OnPropertyChanged(nameof(AddNewButtonCalculateIsValid));
      }
    }

    public int AddNewSpellId
    {
      get { return _addNewSpellId; }
      set
      {
        _addNewSpellId = value;
        OnPropertyChanged(nameof(AddNewSpellId));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
        OnPropertyChanged(nameof(AddNewButtonCalculateIsValid));
      }
    }

    public int AddNewSpellCount
    {
      get { return _addNewSpellCount; }
      set
      {
        _addNewSpellCount = value;
        OnPropertyChanged(nameof(AddNewSpellCount));
        OnPropertyChanged(nameof(AddNewButtonAddIsValid));
        OnPropertyChanged(nameof(AddNewButtonCalculateIsValid));
      }
    }

    public int[] SpellCounts
    {
      get { return Enumerable.Range(0, 20).ToArray(); }
    }

    public bool AddNewButtonAddIsValid
    {
      get
      {
        switch (AddNewDialogTab)
        {
          case 0: // item
            return AvailableItems.Contains(AddNewItemId) && AvailableLevel(AddNewItemId, AddNewItemLevel) &&
                   ItemRequirementTypeEnum.EnumValues.Contains(AddNewItemRequirementType);
          case 1: // artifact
            return ArtifactItems.Contains(AddNewArtifactId) && ArtifactLevels.Contains(AddNewArtifactLevel);
          case 2: // modifier
            return ModifierItems.Contains(AddNewModifierId) &&
                   ModifierCounts.Contains(AddNewModifierCount);
          case 3: // spell
            return SpellItems.Contains(AddNewSpellId) && SpellCounts.Contains(AddNewSpellCount);
        }

        return false;
      }
    }

    public bool AddNewButtonCalculateIsValid
    {
      get
      {
        return ModifierItems.Contains(AddNewModifierId);
      }
    }


    public bool ContainsItem(int id, int level)
    {
      Dictionary<int, LevelEditorRequirementItem> levels;
      if (_itemsMap.TryGetValue(id, out levels))
      {
        return levels.ContainsKey(level);
      }
      return false;
    }

    public bool ContainsArtifact(int id, int level)
    {
      Dictionary<int, LevelEditorRequirementArtifact> levels;
      if (_artifactsMap.TryGetValue(id, out levels))
      {
        return levels.ContainsKey(level);
      }
      return false;
    }

    public LevelEditorRequirementModifier GetModifier(int id)
    {
      LevelEditorRequirementModifier item;
      _modifiersMap.TryGetValue(id, out item);
      return item;
    }

    private LevelEditorRequirementItem GetItem(int id, int level)
    {
      Dictionary<int, LevelEditorRequirementItem> levels;
      if (!_itemsMap.TryGetValue(id, out levels))
      {
        _itemsMap[id] = levels = new Dictionary<int, LevelEditorRequirementItem>();
      }
      LevelEditorRequirementItem item;
      if (levels.TryGetValue(level, out item))
      {
        return item;
      }
      return null;
    }

    private bool AvailableLevel(int id, int level)
    {
      return _cellLevels.ContainsKey(id) && _cellLevels[id] >= level;
    }

    public bool AddNewItem(int id, int level, ItemRequirementType requirementType, int? value)
    {
      if (_availableItems == null || !_availableItems.Contains(id) || !AvailableLevel(id, level))
      {
        return false;
      }
      Dictionary<int, LevelEditorRequirementItem> levels;
      if (!_itemsMap.TryGetValue(id, out levels))
      {
        _itemsMap[id] = levels = new Dictionary<int, LevelEditorRequirementItem>();
      }
      LevelEditorRequirementItem item;
      if (!levels.TryGetValue(level, out item))
      {
        levels[level] = item = new LevelEditorRequirementItem();
        item.HistoryContext = HistoryContext;
        item.ItemId = id;
        item.ItemIdLabel = id.ToString();
        if (value != null) item.Value = (int)value;
        item.Level = level;
        item.Type = requirementType;
        UpdateAvailableItems(_availableItems);
        return true;
      }
      else if (value != null)
      {
        item.Value = value.Value;
      }

      return false;
    }

    private LevelEditorRequirementArtifact GetArtifact(int id, int level)
    {
      Dictionary<int, LevelEditorRequirementArtifact> levels;
      if (!_artifactsMap.TryGetValue(id, out levels))
      {
        _artifactsMap[id] = levels = new Dictionary<int, LevelEditorRequirementArtifact>();
      }
      LevelEditorRequirementArtifact item;
      if (levels.TryGetValue(level, out item))
      {
        return item;
      }
      return null;
    }

    public bool AddNewArtifact(int id, int level, int? requirementValue)
    {
      if (!_artifactItems.Contains(id) || !_artifactLevels.Contains(level))
      {
        return false;
      }
      Dictionary<int, LevelEditorRequirementArtifact> levels;
      if (!_artifactsMap.TryGetValue(id, out levels))
      {
        _artifactsMap[id] = levels = new Dictionary<int, LevelEditorRequirementArtifact>();
      }
      LevelEditorRequirementArtifact item;
      if (!levels.TryGetValue(level, out item))
      {
        levels[level] = item = new LevelEditorRequirementArtifact();
        item.CalculateAction = (aid, alevel) => TileGrid.ItemCount(aid, alevel);
        item.HistoryContext = HistoryContext;
        item.ItemId = id;
        if (requirementValue != null) item.Value = requirementValue.Value;
        item.Level = level;
        UpdateArtifacts();
        return true;
      }
      else if (requirementValue != null)
      {
        item.Value = requirementValue.Value;
      }

      return false;
    }

    private void UpdateArtifacts()
    {
      Artifacts.Children.Clear();
      foreach (var source in _artifactsMap.ToArray().OrderBy(i => i.Key))
      {
        foreach (var item in source.Value)
        {
          Artifacts.Children.Add(item.Value);
        }
      }
    }

    public bool AddNewModifier(int id, int count)
    {
      if (!_modifierItems.Contains(id))
      {
        return false;
      }

      LevelEditorRequirementModifier modifier;
      if (!_modifiersMap.TryGetValue(id, out modifier))
      {
        modifier = new LevelEditorRequirementModifier();
        modifier.CalculateAction = aid => TileGrid.ModifiersCount(aid);
        modifier.HistoryContext = HistoryContext;
        modifier.Id = id;
        modifier.Value = count;
        Modifiers.Children.Add(modifier);
        _modifiersMap[id] = modifier;
        OnPropertyChanged(nameof(ModifierItems));
        return true;
      }

      modifier.Value = count;

      return false;
    }

    private bool AddNewSpell(int id, int count, bool isRequired)
    {
      if (!_spellItems.Contains(id)) return false;
      LevelEditorRequirementSpell spell;
      if (!_spellMap.TryGetValue(id, out spell))
      {
        spell = new LevelEditorRequirementSpell();
        spell.HistoryContext = HistoryContext;
        spell.ItemId = id;
        spell.Level = 0;
        spell.Value = count;
        spell.ItemChecked = isRequired;
        Spells.Children.Add(spell);
        _spellMap[id] = spell;
        OnPropertyChanged(nameof(SpellItems));
        return true;
      }

      spell.Value = count;

      return false;
    }

    public void UpdateAvailableItems(int[] availableItems)
    {
      var items = AppSettings.Setting.Items.Where(i => i.Type == ItemType.UniversalSwapCell).Select(i => i.Id)
        .Concat(availableItems);
      var set = new HashSet<int>(items);
      _availableItems = set.ToArray();
      Array.Sort(_availableItems);

      foreach (var availableItem in availableItems)
      {
        Dictionary<int, LevelEditorRequirementItem> levels;
        if (!_itemsMap.TryGetValue(availableItem, out levels))
        {
          _itemsMap[availableItem] = levels = new Dictionary<int, LevelEditorRequirementItem>();
        }
        LevelEditorRequirementItem item;
        if (!levels.TryGetValue(0, out item))
        {
          levels[0] = item = new LevelEditorRequirementItem();
          item.HistoryContext = HistoryContext;
          item.ItemId = availableItem;
          item.Level = 0;
          item.Value = 0;
          item.ItemIdLabel = availableItem.ToString();
        }
        foreach (var level in levels)
        {
          level.Value.IsEnabled = true;
        }
      }
      foreach (var pair in _itemsMap)
      {
        if (!availableItems.Contains(pair.Key))
        {
          foreach (var item in pair.Value)
          {
            item.Value.IsEnabled = false;
          }
        }
      }
      Items.Children.Clear();
      foreach (var source in _itemsMap.ToArray().OrderBy(i => i.Key))
      {
        foreach (var item in source.Value)
        {
          Items.Children.Add(item.Value);
        }
      }

      OnPropertyChanged(nameof(AvailableItems));
      OnPropertyChanged(nameof(AddNewButtonAddIsValid));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void DigitalInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }

    private void SwapInput_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      OnPropertyChanged(nameof(SwapIsValid));
    }

    private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
    {
      if ((bool)eventargs.Parameter == false) return;

      switch (AddNewDialogTab)
      {
        case 0: // items
          AddNewItem(AddNewItemId, AddNewItemLevel, AddNewItemRequirementType, null);
          break;
        case 1: // artifact
          AddNewArtifact(AddNewArtifactId, AddNewArtifactLevel, 0);
          break;
        case 2: // modifiers
          AddNewModifier(AddNewModifierId, AddNewModifierCount);
          break;
        case 3: // spell
          AddNewSpell(AddNewSpellId, AddNewSpellCount, false);
          break;
      }
    }

    public void SetRequirements(LevelRequirementDescription[] levelRequirements)
    {
      foreach (var requirement in levelRequirements)
      {
        switch (requirement.Type)
        {
          case LevelRequirementType.CollectItem:
          case LevelRequirementType.GenerateItem:
            if (AppSettings.Setting.GetItemDescription(requirement.Id).Type == ItemType.Artifact)
            {
              AddNewArtifact(requirement.Id, requirement.Level, requirement.Value);
              GetArtifact(requirement.Id, requirement.Level).ItemChecked = true;
            }
            else
            {
              AddNewItem(requirement.Id, requirement.Level, requirement.Type == LevelRequirementType.CollectItem ? ItemRequirementType.Collect : ItemRequirementType.Generate, requirement.Value);
              GetItem(requirement.Id, requirement.Level).ItemChecked = true;
            }
            break;
          case LevelRequirementType.Modifier:
            AddNewModifier(requirement.Id, requirement.Value);
            GetModifier(requirement.Id).IsChecked = true;
            break;
          case LevelRequirementType.Stars:
            if (requirement.Level == 1) Star1.Text = requirement.Value.ToString();
            else if (requirement.Level == 2) Star2.Text = requirement.Value.ToString();
            else if (requirement.Level == 3) Star3.Text = requirement.Value.ToString();
            break;
          case LevelRequirementType.UseSpell:
            AddNewSpell(requirement.Id, requirement.Value, true);
            break;
        }
      }
    }
  }
}
