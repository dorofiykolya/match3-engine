using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Descriptions.Items;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.CombinationActivators
{
  public class UniversalSwapItemActivator : CombinationActivator
  {
    public override void Activate(EngineState engineState, IActivatorContext context, Tile tile, Tile initiator, Swap swap, ActivationResult result)
    {
      if (tile.IsEmpty || initiator.IsEmpty) throw new InvalidOperationException("невозможно произвести рачеты");

      if (swap != null)
      {
        var first = engineState.TileGrid.GetTile(swap.First);
        var second = engineState.TileGrid.GetTile(swap.Second);
        if (first.ItemType == ItemType.UniversalSwapCell)
        {
          initiator = second;
        }
        else
        {
          initiator = first;
        }
      }
      else if (initiator.ItemType == ItemType.UniversalSwapCell)
      {
        var t = initiator;
        initiator = tile;
        tile = t;
      }

      if (tile.Position == initiator.Position) ActivateByNear(engineState, context, tile, result);
      else if (initiator.ItemType == ItemType.UniversalSwapCell) ActivateWithLevel2(engineState, context, tile, initiator, result);
      else if (initiator.Item.Level == LevelId.L0) ActivateWithLevel0(engineState, context, tile, initiator, result);
      else if (initiator.Item.Level == LevelId.L1) ActivateWithLevel1(engineState, context, tile, initiator, result);
      if (initiator.ItemType != ItemType.UniversalSwapCell)
      {
        context.Activate(initiator.Position, initiator.Position, null, result);
      }
    }

    private void ActivateWithLevel2(EngineState engineState, IActivatorContext context, Tile tile, Tile initiator, ActivationResult result)
    {
      context.CheckPosition(tile.Position);
      foreach (var current in engineState.TileGrid.Tiles)
      {
        if (!current.IsEmpty && current != tile)
        {
          context.Activate(current.Position, initiator.Position, null, result);
        }
      }
    }

    private void ActivateWithLevel1(EngineState engineState, IActivatorContext context, Tile tile, Tile initiator, ActivationResult result)
    {
      context.CheckPosition(tile.Position);

      IncreaseLevelEvent evt = null;
      if (engineState.Configuration.Environment.IsGenerateOutputEvents())
      {
        evt = engineState.Output.EnqueueByFactory<IncreaseLevelEvent>(engineState.Tick);
      }

      var initiatorItem = initiator.Item;
      foreach (var current in engineState.TileGrid.Tiles)
      {
        if (current != tile && !current.IsEmpty)
        {
          if (current.Item.Id == initiatorItem.Id && current.Item.Level == LevelId.L0)
          {
            current.SetNextItem(current.Item.CopyAndIncreaseLevel());
            current.ApplyNextItem();

            if (evt != null)
            {
              evt.Items.Add(new IncreaseLevelEvent.IncreaseItem
              {
                Position = current.Position,
                Item = current.Item
              });
            }
          }
        }
      }

      foreach (var current in engineState.TileGrid.Tiles)
      {
        if (current != tile && !current.IsEmpty)
        {
          if (current.Item.Id == initiatorItem.Id)
          {
            context.Activate(current.Position, initiator.Position, null, result);
          }
        }
      }
    }

    private void ActivateWithLevel0(EngineState engineState, IActivatorContext context, Tile tile, Tile initiator, ActivationResult result)
    {
      context.CheckPosition(tile.Position);

      foreach (var current in engineState.TileGrid.Tiles)
      {
        if (current != tile && !current.IsEmpty)
        {
          if (current.Item.Id == initiator.Item.Id)
          {
            context.Activate(current.Position, initiator.Position, null, result);
          }
        }
      }
    }

    private void ActivateByNear(EngineState engineState, IActivatorContext context, Tile tile, ActivationResult result)
    {
      var left = tile.LeftTile;
      var right = tile.RightTile;
      var top = tile.TopTile;
      var bottom = tile.BottomTile;
      var items = new List<Tile>(4);
      if (left != null) items.Add(left);
      if (right != null) items.Add(right);
      if (top != null) items.Add(top);
      if (bottom != null) items.Add(bottom);
      var index = engineState.GetNextRandom(items.Count);

      Tile initiator;

      if (items.Count == 0)
      {
        var findTile = engineState.TileGrid.Tiles.FirstOrDefault(i => !i.IsEmpty && i.ItemType != ItemType.UniversalSwapCell);
        if (findTile != null)
        {
          initiator = findTile;
        }
        else
        {
          initiator = tile;
        }
      }
      else
      {
        initiator = items[index];
      }

      if (initiator == null) throw new NullReferenceException("initiator не может быть null");

      if (initiator.ItemType == ItemType.UniversalSwapCell) ActivateWithLevel2(engineState, context, tile, initiator, result);
      else if (initiator.Item.Level == LevelId.L0) ActivateWithLevel0(engineState, context, tile, initiator, result);
      else if (initiator.Item.Level == LevelId.L1) ActivateWithLevel1(engineState, context, tile, initiator, result);
    }
  }
}
