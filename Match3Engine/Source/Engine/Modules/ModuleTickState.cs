using System.Collections.Generic;
namespace Match3.Engine.Modules
{
  public class ModuleTickState
  {
    private readonly HashSet<TickInvalidation> _map = new HashSet<TickInvalidation>();

    public void Invalidate(TickInvalidation value)
    {
      _map.Add(value);
    }

    public bool IsInvalid(TickInvalidation value)
    {
      return _map.Contains(value);
    }

    public bool IsValid(TickInvalidation value)
    {
      return !_map.Contains(value);
    }

    public bool IsValid()
    {
      return _map.Count == 0;
    }

    public bool IsValid(params TickInvalidation[] value)
    {
      foreach (var invalidation in value)
      {
        if (!IsValid(invalidation)) return false;
      }
      return true;
    }

    public void Validate()
    {
      _map.Clear();
    }
  }
}
