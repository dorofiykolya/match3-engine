using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.Properties;
using Match3.Settings;

namespace Match3.Editor
{
  public class AppSettings
  {
    static AppSettings()
    {
      var bytes = File.ReadAllBytes("Source/Match3Settings.xml");
      Setting = Match3SettingsParser.Parse(bytes);
    }

    public static Match3Setting Setting { get; private set; }
  }
}
