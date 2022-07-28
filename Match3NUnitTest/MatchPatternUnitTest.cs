using NUnit.Framework;
using Match3.Engine.Utils;

namespace Match3NUnitTest
{
  [TestFixture]
  public class MatchPatternUnitTest
  {
    [Test]
    public void TestCase()
    {
      var result = PatternParser.ParseToCell(@"[ | |#]" +
                                              "[ | |#]" +
                                              "[#|#|#]");
      //                   x  y
      Assert.IsTrue(result[0, 0] == PatternParser.Cell.Empty);
      Assert.IsTrue(result[1, 0] == PatternParser.Cell.Empty);
      Assert.IsTrue(result[2, 0] == PatternParser.Cell.Fill);

      Assert.IsTrue(result[0, 1] == PatternParser.Cell.Empty);
      Assert.IsTrue(result[1, 1] == PatternParser.Cell.Empty);
      Assert.IsTrue(result[2, 1] == PatternParser.Cell.Fill);

      Assert.IsTrue(result[0, 2] == PatternParser.Cell.Fill);
      Assert.IsTrue(result[1, 2] == PatternParser.Cell.Fill);
      Assert.IsTrue(result[2, 2] == PatternParser.Cell.Fill);

    }
  }
}
