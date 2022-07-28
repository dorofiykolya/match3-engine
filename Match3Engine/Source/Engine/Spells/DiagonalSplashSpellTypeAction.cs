using System;
using Match3.Engine.Levels;
using Match3.Engine.OutputEvents;

namespace Match3.Engine.Spells
{
  public class DiagonalSplashSpellTypeAction : SpellTypeAction
  {
    public override void Execute(EngineState state, UseSpell useSpell)
    {
      var isGenerateOutputEvents = state.Environment.IsGenerateOutputEvents();

      var grid = state.TileGrid;
      var tile = grid.GetTile(useSpell.Positions[0]);
      if (tile == null) throw new ArgumentException("ячейка не существует");
      if (tile.IsEmpty) throw new ArgumentException("ячейка пуста");

      var activator = state.TileGridActivator;
      var bounds = grid.Bounds;

      ActivationResult activationResult = null;
      UseSpellActionEvent useSpellActionEvent = null;
      if (isGenerateOutputEvents)
      {
        activationResult = new ActivationResult();
        useSpellActionEvent = state.Output.EnqueueByFactory<UseSpellActionEvent>(state.Tick);
        useSpellActionEvent.UseSpell = useSpell;
      }

      //left-top
      var x = tile.Position.X;
      var y = tile.Position.Y;
      while (x >= bounds.MinX && y >= bounds.MinY)
      {
        var position = new Point(x, y);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(position);
        }

        activator.Activate(position, activationResult);
        --x;
        --y;
      }

      //right-bottom
      x = tile.Position.X;
      y = tile.Position.Y;
      while (x <= bounds.MaxX && y <= bounds.MaxY)
      {
        var position = new Point(x, y);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(position);
        }

        activator.Activate(position, activationResult);
        ++x;
        ++y;
      }

      //left-bottom
      x = tile.Position.X;
      y = tile.Position.Y;
      while (x >= bounds.MinX && y <= bounds.MaxY)
      {
        var position = new Point(x, y);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(position);
        }

        activator.Activate(position, activationResult);
        --x;
        ++y;
      }

      //right-top
      x = tile.Position.X;
      y = tile.Position.Y;
      while (x <= bounds.MaxX && y >= bounds.MinY)
      {
        var position = new Point(x, y);

        if (isGenerateOutputEvents)
        {
          useSpellActionEvent.ActivateTiles.Add(position);
        }

        activator.Activate(position, activationResult);
        ++x;
        --y;
      }

      if (isGenerateOutputEvents)
      {
        state.Output.EnqueueByFactory<ActivateEvent>(state.Tick).InitializeFrom(activationResult.Queue);
      }
    }
  }
}
