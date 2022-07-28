using System.Text;
using Match3.Properties;

namespace Match3.Settings
{
  public static class Match3SettingResources
  {
    private static Match3Setting _setting;

    static Match3SettingResources()
    {
      _setting = Match3SettingsParser.Parse(Encoding.UTF8.GetBytes(Resource.Match3Settings));
    }

    public static Match3Setting Setting { get { return _setting; } }
  }
}
