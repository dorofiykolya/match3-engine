using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Modules
{
  public class GeneratorModule : EngineModule
  {
    public override void Tick(Engine engine, int currentTick, EngineState state, ModuleTickState tickState, int tickStep)
    {
      if (!state.IsGeneratorStoped)
      {
        var grid = state.TileGrid;
        var generator = grid.Generator;
        GenerateEvent evt = null;
        foreach (var input in grid.Inputs)
        {
          var tile = input.GetTile(input.Direction);
          if (tile != null && tile.IsEmpty && tile.IsMovable)
          {
            var item = generator.GenerateItem(input);
            tile.SetItem(item);

            if (engine.Environment.IsGenerateOutputEvents())
            {
              if (evt == null)
              {
                evt = engine.EnqueueByFactory<GenerateEvent>(currentTick);
              }
              evt.Items.Add(new GenerateEvent.GenerateItem
              {
                FromEdge = input.Direction.Invert(),
                ToTile = tile.Position,
                Item = item.Copy()
              });
            }

            state.Invalidate();
            tickState.Invalidate(TickInvalidation.Generation);
          }
        }
      }
    }
  }
}