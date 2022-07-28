using NUnit.Framework;
using System;
using System.Collections.Generic;
using Match3.Engine;
using Match3.Engine.InputActions;
using Match3.Engine.Levels;
using Match3Debug;

namespace Match3NUnitTest
{
  [TestFixture]
  public class Match3EngineUnitTest
  {
    private Engine _engine;
    private Configuration _configuration;

    [Test]
    public void TestCreate()
    {
      var level = TestConfigurationFactory.GetLevelDescription();
      _configuration = new TestConfigurationFactory().Create(EngineEnvironment.DefaultDebugClient, level, 10, 4, 4);
      _engine = new Engine(_configuration);
    }

    [Test]
    public void TestCreation()
    {
      TestCreate();
      Assert.IsNotNull(_engine);
      Assert.IsNotNull(_engine.State);
    }

    [Test]
    public void TestLevel1()
    {
      TestCreate();
      var swaps = new List<Swap>();
      _engine.State.AvailableSwaps(swaps);
      //Assert.IsTrue(swaps.Contains(new Swap(new Point(1, 0), new Point(1, 1))));
      _engine.FastForward(3);
    }

    [Test]
    public void TestCleanup()
    {

    }
  }
}
