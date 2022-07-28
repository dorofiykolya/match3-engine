namespace Match3.Engine.Levels
{
  public class ItemId
  {
    public const int Empty = 0;

    public const int Green = 1;
    public const int Red = 2;
    public const int White = 3;
    public const int Blue = 4;
    public const int Violet = 5;
    public const int Yellow = 6;
    public const int Universal = 7;

    public const int Artifact = ArtifactId.Artifact;

    public static int[] Ids
    {
      get { return new[] { Green, Red, White, Blue, Violet, Yellow, Universal }; }
    }
  }
}
