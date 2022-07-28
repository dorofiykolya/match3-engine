using System.IO;
using Match3.LevelConverter.MagicCrush;
using Newtonsoft.Json;

namespace Match3.LevelConverter
{
  public class LevelFileLoader
  {
    public MCLevel LoadFromFile(string file)
    {
      return LoadFromJson(File.ReadAllText(file));
    }

    public MCLevel LoadFromJson(string json)
    {
      return JsonConvert.DeserializeObject<MCLevel>(json);
    }
  }
}
