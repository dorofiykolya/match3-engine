using System;
using System.IO;
using System.Text;
using Match3.Settings;
using NUnit.Framework;

namespace Match3NUnitTest
{
  [TestFixture]
  public class Match3SettingsParserUnitTest
  {
    [Test]
    public void TestParser()
    {
      var bytes = File.ReadAllBytes(Path.Combine(TestContext.CurrentContext.TestDirectory, "Source/Match3Settings.xml"));

      var settings = Match3SettingsParser.Parse(bytes);

      Assert.IsNotNull(settings.Modifiers);
      Assert.IsNotEmpty(settings.Modifiers);

      Assert.IsNotNull(settings.Descriptions);
      Assert.IsNotEmpty(settings.Descriptions);

      Assert.IsNotNull(settings.Items);
      Assert.IsNotEmpty(settings.Items);

      Assert.IsNotNull(settings.Spells);
      Assert.IsNotEmpty(settings.Spells);
    }
  }
}
