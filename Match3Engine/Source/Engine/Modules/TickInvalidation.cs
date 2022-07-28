namespace Match3.Engine.Modules
{
  public class TickInvalidation
  {
    public static readonly TickInvalidation Fall = new TickInvalidation("Fall");
    public static readonly TickInvalidation Generation = new TickInvalidation("Generation");
    public static readonly TickInvalidation MatchItems = new TickInvalidation("MatchItems");
    public static readonly TickInvalidation Shuffle = new TickInvalidation("Shuffle");
    public static readonly TickInvalidation Collect = new TickInvalidation("Collect");

    private readonly string _id;
    public TickInvalidation(string id) { _id = id; }
    public string Id { get { return _id; } }
    public override string ToString() { return "TickInvalidation(" + _id + ")"; }
  }
}
