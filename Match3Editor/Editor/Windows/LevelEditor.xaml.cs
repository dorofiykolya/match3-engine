using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Match3.Editor.LevelEditor;
using Match3.Editor.Utils;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Providers;
using Match3.Settings;
using Match3Editor.Annotations;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Orientation = Match3.Engine.Levels.Orientation;
using Point = Match3.Engine.Levels.Point;

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для LevelEditor.xaml
  /// </summary>
  public partial class LevelEditor : Window, INotifyPropertyChanged, IHistoryExecuter
  {
    public enum TileIndex
    {
      None,
      Add,
      Remove
    }

    private readonly Dictionary<int, LevelEditorModifierToolbarItem> _modifiers =
      new Dictionary<int, LevelEditorModifierToolbarItem>();

    private Item[] _items;
    private string[] _itemsTooltips;
    private bool _inEvent;
    private HistoryManager _history;
    private readonly LevelEditorModel _model;

    public int Energy = 100;
    public LevelWorkspace Workspace;
    public EdgeType? EdgeType;
    public Item Item;
    public bool NeedDeleteItem;
    public Direction Direction;
    private int? ModifierIndex;
    private TileIndex TileIndexValue;
    private PositionConverter _positionConverter = new PositionConverter();

    public LevelEditor()
    {
      _model = new LevelEditorModel(AppSettings.Setting, this);
      _history = HistoryManager.Get(HistoryContext);

      InitializeComponent();

      Closed += (sender, args) => _history.Dispose();

      LoadSettings(AppSettings.Setting);

      GenerateLevel();

      TileGridControl.Items.Visibility = Visibility.Visible;
      TileGridControl.Directions.Visibility = Visibility.Hidden;
      TileGridControl.PositionItem += TileGridControlOnPositionItem;
      TileGridControl.EdgePositionItem += TileGridControlOnEdgePositionItem;
      TileGridControl.InitializeTiles();
    }

    public void LoadLevel(LevelDescription level, string file)
    {
      _history.Clear();

      if (file != null && File.Exists(file))
      {
        Workspace = new LevelWorkspace(file);
      }

      TileGridControl.SetContent(level);

      _model.LevelRandomSeed = level.RandomSeed;

      AvailableItems.Available(level.AvailableItems);

      Requirements.Swaps = level.Swaps;
      Requirements.SetRequirements(level.Requirements);
      Requirements.UpdateAvailableItems(level.AvailableItems);
    }

    public void GenerateLevel(int width = 7, int height = 7)
    {
      var grid = _model.GenerateLevel(width, height, AvailableItems.AvailableItems);
      var level = grid.ToLevelDescription();
      level.Swaps = 20;

      TileGridControl.SetContent(level);
      Requirements.Swaps = 20;
    }

    [Bindable(true, BindingDirection.OneWay)]
    public DependencyObject HistoryContext
    {
      get { return this; }
      set { }
    }

    [Bindable(true)]
    public bool CanUndo
    {
      get { return _history.CanUndo; }
    }

    [Bindable(true)]
    public bool CanRedo
    {
      get { return _history.CanRedo; }
    }

    [Bindable(true)]
    public string UndoDescription
    {
      get { return _history.UndoDescription; }
    }

    [Bindable(true)]
    public string RedoDescription
    {
      get { return _history.RedoDescription; }
    }

    public bool IsEditItems
    {
      get { return TileGridControl == null || TileGridControl.Items.Visibility == Visibility.Visible; }
    }

    public void Execute(Action redo, Action undo, string description)
    {
      _history.Execute(() =>
      {
        redo();
        CustomSnackbar.MessageQueue.Enqueue("DO: " + description);
      }, () =>
      {
        undo();
        CustomSnackbar.MessageQueue.Enqueue("UNDO: " + description);
      }, description);
      NotifyHistory();
    }

    public void Undo()
    {
      _history.Undo();
      NotifyHistory();
    }

    public void Redo()
    {
      _history.Redo();
      NotifyHistory();
    }

    private void LoadSettings(Match3Setting setting)
    {
      var items = new List<Item>();
      var tooltips = new List<string>();

      foreach (var item in setting.Items)
      {
        var i = 0;
        foreach (var level in item.Levels)
        {
          tooltips.Add(
            $"item (id:{item.Id}, level:{i}, score:{level.Score}, type:{item.Type}){Environment.NewLine}item description: \"{setting.GetDescription(item.Description)}\"{Environment.NewLine}level description: \"{setting.GetDescription(level.Description)}\"");
          items.Add(new Item
          {
            Id = item.Id,
            Level = i
          });
          ++i;
        }
      }

      _itemsTooltips = tooltips.ToArray();
      _items = items.ToArray();

      {
        var i = 0;
        foreach (var item in _items)
        {
          var button = new LevelEditorToolItem();
          button.ToolTip = _itemsTooltips[i];
          button.Label.Content = $"{item.Id}:{item.Level + 1}";
          button.BackgroundColor.Fill = ItemToColor.ToColor(item.Id);
          ItemsToolbar.Items.Add(button);
          i++;
        }
      }

      foreach (var modifier in setting.Modifiers)
      {
        var addItem = new LevelEditorModifierToolbarItem
        {
          Background = ModifierTypeToColor.ToColor(setting.GetModifierType(modifier.Id)),
          Label = modifier.Id.ToString(),
          Id = modifier.Id,
          Type = LevelEditorModifierToolbarItem.ActionType.Add,
          ToolTip = modifier.Type.ToString()
        };

        _modifiers[ModifiersToolbar.Items.Count] = addItem;
        ModifiersToolbar.Items.Add(addItem);

        var removeItem = new LevelEditorModifierToolbarItem
        {
          Background = ModifierTypeToColor.ToColor(setting.GetModifierType(modifier.Id)),
          Label = modifier.Id.ToString(),
          Id = modifier.Id,
          Type = LevelEditorModifierToolbarItem.ActionType.Remove,
          ToolTip = modifier.Type.ToString()
        };

        _modifiers[ModifiersToolbar.Items.Count] = removeItem;
        ModifiersToolbar.Items.Add(removeItem);
      }

      AvailableItems.Load(items.Where(i => setting.GetItemDescription(i.Id).Type == ItemType.Cell).Select(i => i.Id)
        .Distinct().ToArray());
      Requirements.UpdateAvailableItems(AvailableItems.AvailableItems);
    }

    private void EdgesToolbar_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!IsLoaded || _inEvent) return;
      _inEvent = true;
      var index = EdgesToolbar.SelectedIndex;
      if (index == -1) EdgesToolbar.SelectedIndex = 0;
      if (index == -1 || index == 0) EdgeType = null;
      else EdgeType = (EdgeType)(index - 1);
      Item = null;
      ItemsToolbar.SelectedIndex = 0;
      ModifiersToolbar.SelectedIndex = 0;
      TilesToolbar.SelectedIndex = 0;
      TileIndexValue = TileIndex.None;
      NeedDeleteItem = false;
      NotifyHistory();
      _inEvent = false;
    }

    private void ItemsToolbar_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!IsLoaded || _inEvent) return;
      _inEvent = true;
      var index = ItemsToolbar.SelectedIndex;
      if (index == -1) ItemsToolbar.SelectedIndex = 0;
      if (index == -1 || index == 0)
      {
        NeedDeleteItem = false;
        Item = null;
      }
      else if (index == 1)
      {
        Item = null;
        NeedDeleteItem = true;
      }
      else
      {
        NeedDeleteItem = false;
        Item = _items[index - 2];
      }

      EdgeType = null;
      EdgesToolbar.SelectedIndex = 0;
      ModifiersToolbar.SelectedIndex = 0;
      TilesToolbar.SelectedIndex = 0;
      TileIndexValue = TileIndex.None;
      NotifyHistory();
      _inEvent = false;
    }

    private void ModifiersToolbar_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!IsLoaded || _inEvent) return;
      _inEvent = true;
      EdgesToolbar.SelectedIndex = 0;
      ItemsToolbar.SelectedIndex = 0;
      TilesToolbar.SelectedIndex = 0;
      TileIndexValue = TileIndex.None;
      var index = ModifiersToolbar.SelectedIndex;
      if (index == 0)
      {
        ModifierIndex = null;
      }
      else if (index == 1)
      {
        ModifierIndex = -1;
      }
      else
      {
        LevelEditorModifierToolbarItem item;
        if (_modifiers.TryGetValue(index, out item) && item != null)
        {
          ModifierIndex = index;
        }
      }

      _inEvent = false;
    }

    private void TileToolbar_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!IsLoaded || _inEvent) return;
      _inEvent = true;
      EdgesToolbar.SelectedIndex = 0;
      ItemsToolbar.SelectedIndex = 0;
      ModifiersToolbar.SelectedIndex = 0;
      var index = TilesToolbar.SelectedIndex;
      if (index == 0)
      {
        TileIndexValue = TileIndex.None;
      }
      else if (index == 1)
      {
        TileIndexValue = TileIndex.Add;
      }
      else if (index == 2)
      {
        TileIndexValue = TileIndex.Remove;
      }

      _inEvent = false;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyHistory()
    {
      OnPropertyChanged(nameof(CanRedo));
      OnPropertyChanged(nameof(CanUndo));
      OnPropertyChanged(nameof(UndoDescription));
      OnPropertyChanged(nameof(RedoDescription));
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void MenuItem_Undo_OnClick(object sender, RoutedEventArgs e)
    {
      Undo();
    }

    private void MenuItem_Redo_OnClick(object sender, RoutedEventArgs e)
    {
      Redo();
    }

    private void AvailableItems_OnChanged()
    {
      Requirements.UpdateAvailableItems(AvailableItems.AvailableItems);
    }

    private void Button_Play_OnClick(object sender, RoutedEventArgs e)
    {
      LevelEditorUtils.PlayLevel(Serialize());
    }

    private LevelDescription Serialize()
    {
      return _model.Serialize(TileGridControl.SerializeItems(), TileGridControl.SerializeEdges(), Requirements.Swaps,
        Requirements.Requirements);
    }

    private void Requirements_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    private void ShowItemsHandler(object sender, RoutedEventArgs e)
    {
      if (_inEvent || TileGridControl == null) return;
      Execute(() =>
      {
        TileGridControl.Items.Visibility = Visibility.Visible;
        TileGridControl.Directions.Visibility = Visibility.Hidden;
        _inEvent = true;
        ToggleEditor.IsChecked = false;
        _inEvent = false;
      }, () =>
      {
        TileGridControl.Items.Visibility = Visibility.Hidden;
        TileGridControl.Directions.Visibility = Visibility.Visible;
        _inEvent = true;
        ToggleEditor.IsChecked = true;
        _inEvent = false;
      }, "editor switch to item-editor");

    }

    private void ShowDirectionsHandler(object sender, RoutedEventArgs e)
    {
      if (_inEvent || TileGridControl == null) return;
      Execute(() =>
      {
        TileGridControl.Items.Visibility = Visibility.Hidden;
        TileGridControl.Directions.Visibility = Visibility.Visible;
        _inEvent = true;
        ToggleEditor.IsChecked = true;
        _inEvent = false;
      }, () =>
      {
        TileGridControl.Items.Visibility = Visibility.Visible;
        TileGridControl.Directions.Visibility = Visibility.Hidden;
        _inEvent = true;
        ToggleEditor.IsChecked = false;
        _inEvent = false;
      }, "editor switch to direction-editor");
    }

    private void DirectionsToolbar_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (DirectionsToolbar.SelectedIndex == -1)
      {
        Direction = Direction.None;
      }
      else if (DirectionsToolbar.SelectedIndex == 0)
      {
        Direction = Direction.Bottom;
      }
      else if (DirectionsToolbar.SelectedIndex == 1)
      {
        Direction = Direction.Top;
      }
      else if (DirectionsToolbar.SelectedIndex == 2)
      {
        Direction = Direction.Left;
      }
      else if (DirectionsToolbar.SelectedIndex == 3)
      {
        Direction = Direction.Right;
      }
    }

    private void TileGridControlOnPositionItem(Point point)
    {
      if (IsEditItems)
      {
        if (ModifiersToolbar.SelectedIndex != 0)
        {
          var tile = TileGridControl.GetTile(point);
          if (tile != null && ModifierIndex != null)
          {
            if (ModifierIndex == -1)
            {
              tile.RemoveAllModifiers();
            }
            else
            {
              var item = _modifiers[ModifierIndex.Value];
              if (item.Type == LevelEditorModifierToolbarItem.ActionType.Add)
              {
                tile.AddModifier(item.Id);
              }
              else if (item.Type == LevelEditorModifierToolbarItem.ActionType.Remove)
              {
                tile.RemoveModifier(item.Id);
              }
            }
          }
        }
        else if (TileIndexValue != TileIndex.None)
        {
          if (TileIndexValue == TileIndex.Add)
          {
            var tile = TileGridControl.GetTile(point);
            if (tile == null)
            {
              Execute(
                () => { TileGridControl.AddTile(point); },
                () => { TileGridControl.RemoveTile(point); },
                $"add tile: '{point}'");
            }
          }
          else if (TileIndexValue == TileIndex.Remove)
          {
            var tile = TileGridControl.GetTile(point);
            if (tile != null)
            {
              var lastItem = TileGridControl.GetItem(point);
              var lastDirection = TileGridControl.GetDirection(point).Direction;
              var edges = new List<LevelEdgeDescription>();

              foreach (var direction in new[] { Direction.Left, Direction.Right, Direction.Top, Direction.Bottom })
              {
                var edgePosition = _positionConverter.TileToEdge(point, direction);
                LevelEdgeItem edge = TileGridControl.GetEdge(edgePosition);
                if (edge != null)
                {
                  LevelEdgeDescription info = new LevelEdgeDescription
                  {
                    Type = edge.Type,
                    Direction = edge.Direction,
                    Index = edge.Index,
                    Queue = edge.Queue,
                    Position = edgePosition
                  };
                  edges.Add(info);
                }
              }

              Execute(() =>
                {
                  TileGridControl.RemoveItem(point);
                  TileGridControl.RemoveTile(point);
                }, () =>
                {
                  TileGridControl.AddTile(point);
                  TileGridControl.AddItem(lastItem);
                  TileGridControl.GetDirection(point).Direction = lastDirection;
                  foreach (var edge in edges)
                  {
                    TileGridControl.GetEdge(edge.Position).SetEdge(edge);
                  }
                }, $"remove tile: '{point}'");
            }
          }
        }
        else if (Item != null)
        {
          Execute(() =>
            {
              TileGridControl.AddItem(new LevelTileDescription
              {
                Direction = Direction.Bottom,
                Item = Item,
                Type = TileType.Movable,
                Position = point,
                Modifiers = new Modifier[0]
              });
            }, () => { TileGridControl.RemoveItem(point); },
            $"add item{Environment.NewLine}position:'{point}'{Environment.NewLine}item:'{Item}'");
          TileGridControl.ResetLastPosition();
        }
        else
        {
          var item = TileGridControl.GetItem(point);
          if (item != null)
          {
            if (NeedDeleteItem)
            {
              var lastTile = TileGridControl.GetItem(point);
              Execute(() => { TileGridControl.RemoveItem(point); }, () => { TileGridControl.AddItem(lastTile); },
                $"remove item: {point}, id:{item.Item.Id}, level:{item.Item.Level}");

              TileGridControl.ResetLastPosition();
            }
            else if (Item != null)
            {
              var lastItem = item.Item;
              Execute(() => { item.SetContent(Item); }, () => { item.SetContent(lastItem); },
                $"change item:'{lastItem}'->'{Item}'");

              TileGridControl.ResetLastPosition();
            }
          }
        }
      }
      else
      {
        if (Direction == Direction.Bottom || Direction == Direction.Top || Direction == Direction.Left ||
            Direction == Direction.Right)
        {
          var directionItem = TileGridControl.GetDirection(point);
          if (directionItem != null)
          {
            var lastDirection = directionItem.Direction;
            if (lastDirection != Direction)
            {
              Execute(() => { directionItem.Direction = Direction; },
                () => { directionItem.Direction = lastDirection; },
                $"change tile direction: '{lastDirection}'->'{Direction}'");
              directionItem.Direction = Direction;
              TileGridControl.ResetLastPosition();
            }
          }
        }
      }
    }

    private void TileGridControlOnEdgePositionItem(Match3.Engine.Levels.Point point)
    {
      var converter = new PositionConverter();
      var edge = TileGridControl.GetEdge(point);
      if (edge != null)
      {
        if (EdgeType.HasValue)
        {
          var lastType = edge.Type;
          if (lastType != EdgeType.Value)
          {
            Execute(() => { edge.Type = EdgeType.Value; }, () => { edge.Type = lastType; },
              $"change edge type'{lastType}'->'{EdgeType.Value}'");
          }

          var newDirection = edge.Direction;
          if (!newDirection.IsValidDirection() ||
              TileGridControl.GetTile(converter.EdgeToTile(point, newDirection)) == null)
          {
            if (converter.GetEdgeOrientationByPosition(point) == Orientation.Horizontal)
            {
              if (TileGridControl.GetTile(converter.EdgeToTile(point, Direction.Left)) != null)
                newDirection = Direction.Left;
              else newDirection = Direction.Right;
            }
            else
            {
              if (TileGridControl.GetTile(converter.EdgeToTile(point, Direction.Top)) != null)
                newDirection = Direction.Top;
              else newDirection = Direction.Bottom;
            }
          }

          edge.Direction = newDirection;
          TileGridControl.ResetLastEdgePosition();
        }
        else if (edge.Type != Engine.Levels.EdgeType.None)
        {
          Opacity = 0.5;


          var availableDirection = true;
          if (edge.Direction.IsHorizontal())
          {
            var left = converter.EdgeToTile(edge.Position, Direction.Left);
            var right = converter.EdgeToTile(edge.Position, Direction.Right);
            availableDirection = TileGridControl.GetTile(left) != null && TileGridControl.GetTile(right) != null;
          }
          else
          {
            var top = converter.EdgeToTile(edge.Position, Direction.Top);
            var down = converter.EdgeToTile(edge.Position, Direction.Bottom);
            availableDirection = TileGridControl.GetTile(top) != null && TileGridControl.GetTile(down) != null;
          }

          var newDirection = edge.Direction;
          if (!newDirection.IsValidDirection() ||
              TileGridControl.GetTile(converter.EdgeToTile(point, newDirection)) == null)
          {
            if (converter.GetEdgeOrientationByPosition(point) == Orientation.Horizontal)
            {
              if (TileGridControl.GetTile(converter.EdgeToTile(point, Direction.Left)) != null)
                newDirection = Direction.Left;
              else newDirection = Direction.Right;
            }
            else
            {
              if (TileGridControl.GetTile(converter.EdgeToTile(point, Direction.Top)) != null)
                newDirection = Direction.Top;
              else newDirection = Direction.Bottom;
            }
          }

          var edit = new LevelEditorEdgeWindow();
          edit.Owner = this;
          edit.EdgeType = edge.Type;
          edit.EdgeDirection = newDirection;
          edit.EdgeIndex = edge.Index;
          edit.AvailableDirection = availableDirection;
          edit.ShowActivated = true;
          edit.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          edit.Apply += (direction, i) =>
          {
            var lastDirection = edge.Direction;
            var lastIndex = edge.Index;
            if (edge.Direction != direction || edge.Index != i)
            {
              Execute(() =>
                {
                  edge.Direction = direction;
                  edge.Index = i;
                }, () =>
                {
                  edge.Direction = lastDirection;
                  edge.Index = lastIndex;
                },
                $"change item:{Environment.NewLine}direction:'{lastDirection}'->'{direction}'{Environment.NewLine}index:'{lastIndex}'->'{i}'");
            }
          };
          edit.ShowDialog();
          TileGridControl.ResetLastEdgePosition();
          Opacity = 1;
        }
      }
    }

    private void MenuItem_Save_OnClick(object sender, RoutedEventArgs e)
    {
      Save();
    }

    private void MenuItem_SaveAs_OnClick(object sender, RoutedEventArgs e)
    {
      SaveAs();
    }

    private void CommandBinding_Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      Save();
    }

    private void CommandBinding_SaveAs_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      SaveAs();
    }

    private void CommandBinding_Undo_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      Undo();
    }

    private void CommandBinding_Redo_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      Redo();
    }

    private void CommandBinding_Reload_OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      Reload();
    }

    private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
    {
      Exit();
    }

    private void MenuItem_History_OnClick(object sender, RoutedEventArgs e)
    {
      var window = new HistoryWindow();
      window.Owner = this;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.Show();
    }

    private void Exit()
    {
      Close();
    }

    private void Shuffle()
    {
      Action action, undo;
      TileGridControl.Shuffle(out action, out undo);
      Execute(action, undo, "shuffle");
    }

    private void Regenerate()
    {
      Action action, undo;
      TileGridControl.Regenerate(out action, out undo, AvailableItems.AvailableItems);
      Execute(action, undo, "regenerate");
    }

    private void Save()
    {
      if (Workspace != null)
      {
        var data = Serialize();
        var json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Workspace.FilePath, json);

        RecentlyFiles.Instance.AddRecently(Workspace.FilePath);
      }
      else
      {
        SaveAs();
      }
    }

    private void SaveAs()
    {
      var dialog = new SaveFileDialog();
      dialog.Title = "Save As...";
      dialog.DefaultExt = ".json";
      dialog.AddExtension = true;
      if (Workspace != null)
      {
        dialog.FileName = Workspace.FileName;
        dialog.InitialDirectory = Workspace.Directory;
      }
      else
      {
        dialog.FileName = "level_01.json";
      }

      dialog.FileOk += (sender, args) =>
      {
        if (!args.Cancel)
        {
          Workspace = new LevelWorkspace();
          Workspace.Directory = System.IO.Path.GetDirectoryName(dialog.FileName);
          Workspace.FileName = System.IO.Path.GetFileName(dialog.FileName);
          Save();
        }
      };
      dialog.ShowDialog(this);
    }

    private void Reload()
    {

    }

    private void Button_Analyze_OnClick(object sender, RoutedEventArgs e)
    {
      var analyze = new AnalyzeWindow();
      analyze.Analyze(Serialize(), Energy);
      analyze.Show();
    }

    private void Button_Shuffle_OnClick(object sender, RoutedEventArgs e)
    {
      Shuffle();
    }

    private void Button_Regen_OnClick(object sender, RoutedEventArgs e)
    {
      Regenerate();
    }
  }
}
