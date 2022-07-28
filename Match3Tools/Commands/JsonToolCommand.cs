using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tectil.NCommand.Contract;

namespace Match3Tools.Commands
{
  public class JsonToolCommand
  {
    [Command("jsonPretty", "format file or folder with json files")]
    public void JsonPretty([Argument("path")]string path, [Argument("filter", "*")]string filter)
    {
      if (File.Exists(path))
      {
        var result = JObject.Parse(File.ReadAllText(path)).ToString(Formatting.Indented);
        File.WriteAllText(path, result);
      }
      else if (Directory.Exists(path))
      {
        var files = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
        foreach (var file in files)
        {
          try
          {
            var result = JObject.Parse(File.ReadAllText(file)).ToString(Formatting.Indented);
            File.WriteAllText(file, result);
          }
          catch (Exception e)
          {
            Console.WriteLine("can't parse file: " + file);
          }
        }
      }
    }

    [Command("jsonCompress", "format file or folder with json files")]
    public void JsonCompress([Argument("path")]string path, [Argument("filter", "*")]string filter)
    {
      if (File.Exists(path))
      {
        var result = JObject.Parse(File.ReadAllText(path)).ToString(Formatting.None);
        File.WriteAllText(path, result);
      }
      else if (Directory.Exists(path))
      {
        var files = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
        foreach (var file in files)
        {
          try
          {
            var result = JObject.Parse(File.ReadAllText(file)).ToString(Formatting.None);
            File.WriteAllText(file, result);
          }
          catch (Exception e)
          {
            Console.WriteLine("can't parse file: " + file);
          }
        }
      }
    }
  }
}
