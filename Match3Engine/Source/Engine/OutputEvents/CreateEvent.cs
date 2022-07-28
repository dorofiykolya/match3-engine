using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;

namespace Match3.Engine.OutputEvents
{
  public class CreateEvent : OutputEvent
  {
    public TileInfo[] Tiles;
    public EdgeInfo[] Edges;
    public RequirementInfo[] Requirements;
    public Spell[] Spells;
    public int Energy;
    public int Swaps;
    public int Score;

    public override void Reset()
    {
      Tiles = null;
      Edges = null;
      Requirements = null;
      Energy = 0;
      Swaps = 0;
      Score = 0;
    }

    public void InitializeFrom(EngineState state)
    {
      var tiles = new List<TileInfo>();
      foreach (var tile in state.TileGrid.Tiles)
      {
        tiles.Add(new TileInfo
        {
          Position = tile.Position,
          Direction = tile.Direction,
          Item = tile.Item != null ? tile.Item.Copy() : null,
          Type = tile.Type,
          Modifies = tile.Modifiers.Select(m => m.Clone()).ToArray()
        });
      }
      Tiles = tiles.ToArray();

      var edges = new List<EdgeInfo>();
      foreach (var edge in state.TileGrid.Edges)
      {
        edges.Add(new EdgeInfo
        {
          Position = edge.Position,
          Direction = edge.Direction,
          Orientation = edge.Orientation,
          Type = edge.Type,
          Index = edge.Index
        });
      }
      Edges = edges.ToArray();

      Energy = state.Energy;
      Swaps = state.MaxSwaps;
      Score = state.Score.Score;

      var requirements = new List<RequirementInfo>();
      foreach (var type in Enum.GetValues(typeof(LevelRequirementType)).Cast<LevelRequirementType>())
      {
        var enumerable = state.Requirement.Remaining(type);
        if (enumerable != null)
        {
          foreach (var requirement in enumerable)
          {
            requirements.Add(new RequirementInfo
            {
              Type = requirement.Type,
              Id = requirement.Id,
              Level = requirement.Level,
              Value = requirement.Value
            });
          }
        }
      }
      Requirements = requirements.ToArray();

      Spells = state.Spells.Spells;
    }

    public class RequirementInfo
    {
      public LevelRequirementType Type;
      public int Id;
      public int Level;
      public int Value;
    }

    public class EdgeInfo
    {
      public Position Position;
      public Direction Direction;
      public Orientation Orientation;
      public EdgeType Type;
      public byte Index;
    }

    public class TileInfo
    {
      public Position Position;
      public Direction Direction;
      public Item Item;
      public TileType Type;
      public Modifier[] Modifies;
    }
  }
}
