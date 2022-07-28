using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Match3.LevelConverter;
using Match3.LevelConverter.MagicCrush;
using Match3.Settings;
using Newtonsoft.Json;

namespace Match3BatchLevelConverter
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("please enter folder and appsettings xml");
        return;
      }
      var folder = args[0];
      if (!Directory.Exists(folder))
      {
        Console.WriteLine("directory not found");
        return;
      }

      var settingsFile = args[1];
      if (!File.Exists(settingsFile))
      {
        Console.WriteLine("app setting file (.xml) not found");
        return;
      }

      var settings = Match3SettingsParser.Parse(File.ReadAllBytes(settingsFile));

      var files = Directory.GetFiles(folder, "*.mapinfo", SearchOption.AllDirectories);
      foreach (var file in files)
      {
        var level = LevelConverter.Convert(settings, JsonConvert.DeserializeObject<MCLevel>(File.ReadAllText(file)));
        File.WriteAllText(Path.ChangeExtension(file, ".bytes"), JsonConvert.SerializeObject(level));
      }

    }
  }
}
