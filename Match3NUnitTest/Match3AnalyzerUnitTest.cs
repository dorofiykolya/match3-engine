using Match3;
using Match3.Analyzer;
using Match3.Engine;
using Match3Debug;
using NUnit.Framework;

namespace Match3NUnitTest
{
  [TestFixture]
  public class Match3AnalyzerUnitTest
  {
    [Test]
    public void TestAnalyzerLevel1()
    {
      var level = TestConfigurationFactory.GetLevelDescription();
      var configuration = new TestConfigurationFactory().Create(EngineEnvironment.DefaultDebugClient, level, 5, 4, 4);

      var factory = new Match3Factory(configuration.Providers, EngineEnvironment.DefaultDebugClient);
      var analyzer = new Match3Analyzer(factory);
      var result = analyzer.Analyze(level, new AnalyzerCancellation());
    }
  }
}
