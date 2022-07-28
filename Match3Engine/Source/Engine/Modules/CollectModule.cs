using Match3.Engine.Descriptions.Items;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Descriptions.Modifiers;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class CollectModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      //if (tickState.IsInvalid(TickInvalidation.MatchItems))
      {
        Process(engine, currentTick, state, tickState, tickStep);
      }
    }

    protected void Process(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      var activators = state.TileGridActivator;
      var grid = state.TileGrid;

      foreach (var tile in grid.Tiles)
      {
        if (!tile.IsEmpty && tile.IsMovable && tile.ItemType == ItemType.Artifact)
        {
          var edge = tile.DirectionEdge;
          if (edge.Direction == tile.Direction.Invert() && edge.Type == EdgeType.Output)
          {
            var item = tile.Item;
            state.Requirement.Dispatch(item.Id, item.Level, LevelRequirementType.CollectItem);
            state.Score.Add(tile.Item);

            tile.SetNextItem(null);
            tile.ApplyNextItem();

            if (engine.Environment.IsGenerateOutputEvents())
            {
              var evt = engine.EnqueueByFactory<ItemToOuputEvent>(currentTick);
              evt.Item = item.Copy();
              evt.Position = tile.Position;
              evt.ToEdge = tile.Direction;

              //engine.EnqueueByFactory<ActivateEvent>(currentTick).Actions.Enqueue(new ActivateEvent.Data
              //{
              //  Item = item,
              //  Position = tile.Position,
              //  Status = ActivateEvent.Status.Activated
              //});
            }

            tickState.Invalidate(TickInvalidation.Collect);
            state.Invalidate();
          }
        }
      }

      var modifierMap = state.Pool.GetModifierActivateData();

      foreach (var pair in activators.Activated)
      {
        var tile = grid.GetTile(pair.Key);
        var item = tile.Item;
        state.Requirement.Dispatch(item.Id, item.Level, LevelRequirementType.CollectItem);
        state.Score.Add(tile.Item);

        tile.SetNextItem(null);
        tile.ApplyNextItem();

        tickState.Invalidate(TickInvalidation.Collect);
        state.Invalidate();

        modifierMap.Set(tile, ModifierActivatorType.Current);

        //near
        foreach (var direction in Point.Directions)
        {
          var nearTile = grid.GetTile(pair.Key + direction);
          if (nearTile != null)
          {
            modifierMap.Set(nearTile, ModifierActivatorType.Near);
          }
        }
      }

      foreach (var pair in modifierMap)
      {
        var tile = pair.Key;
        var activatorType = pair.Value;
        var modifiers = state.Pool.PopList<Modifier>();
        tile.ApplyCollect(activatorType, modifiers);
        foreach (var modifier in modifiers)
        {
          state.Requirement.Dispatch(modifier.Id, modifier.Level, LevelRequirementType.Modifier);
          if (engine.Environment.IsGenerateOutputEvents())
          {
            var evt = engine.EnqueueByFactory<ModifierEvent>(currentTick);
            evt.Id = modifier.Id;
            evt.Level = modifier.Level;
            evt.Position = tile.Position;
          }
        }
        state.Pool.PushList(modifiers);
      }

      modifierMap.Clear();

      foreach (var pair in activators.ToGenerate)
      {
        var tile = grid.GetTile(pair.Key);
        tile.SetNextItem(pair.Value);
        tile.ApplyNextItem();

        var item = pair.Value;
        state.Requirement.Dispatch(item.Id, item.Level, LevelRequirementType.GenerateItem);

        tickState.Invalidate(TickInvalidation.Collect);
        state.Invalidate();
      }
    }
  }
}
