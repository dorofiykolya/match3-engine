using NUnit.Framework;
using System;
using Match3.Engine;
using Match3Debug;
using Match3.Engine.Utils;
using Match3.Engine.Levels;

namespace Match3NUnitTest
{
  [TestFixture]
  public class SwapUnitTest
  {
    [Test]
    public void SwapConstructor()
    {
      var coords = new[]
      {
        new Point(0, 0),
        new Point(1, 0),
        new Point(0, 100),
        new Point(1, 1),
        new Point(100, 5),
      };
      foreach (var offset in Point.Directions)
      {
        foreach (var coord in coords)
        {
          new Swap(coord, coord + offset);
        }
      }
    }

    [Test]
    public void SwapConstructorEqualsPoints()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        new Swap(new Point(), new Point());
      });
    }

    [Test]
    public void SwapConstructorIncorrectPoint()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        new Swap(new Point(), new Point(1, 1));
      });

      Assert.Throws<ArgumentException>(() =>
      {
        new Swap(new Point(), new Point(0, 2));
      });
    }
  }
}
