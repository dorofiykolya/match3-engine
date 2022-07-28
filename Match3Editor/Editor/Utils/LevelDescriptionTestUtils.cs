using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Providers;

namespace Match3.Editor.Utils
{
  public class LevelDescriptionTestUtils
  {
    public static bool Test(LevelDescription level, out Exception exception)
    {
      try
      {
        var providerFactory = new Match3ProviderFactory();
        var provider = providerFactory.Create(AppSettings.Setting);
        var config = new Match3Factory(provider, EngineEnvironment.DefaultDebugClient);
        var engine = config.CreateInstance(level, 1000, new Spell[0]);
        engine.FastForward(10);

        exception = null;
        return true;
      }
      catch (Exception e)
      {
        exception = e;
        return false;
      }
    }
  }
}
