using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Match3.Editor.Windows;
using Match3.Engine.Descriptions.Levels;
using Match3.LevelConverter.MagicCrush;
using Newtonsoft.Json;

namespace Match3.Editor.Utils
{
  public class LevelEditorUtils
  {
    public const string FileExtensionFilter = "all supported files (*.json;*.mapinfo;*.bytes)|*.json;*.mapinfo;*.bytes|(*.json)|*.json|(*.mapinfo)|*.mapinfo|(*.bytes)|*.bytes";

    public static void OpenLevel(LevelDescription level)
    {
      var editor = new Windows.LevelEditor();
      editor.LoadLevel(level, null);
      editor.Show();
    }

    public static void OpenNew(Window owner = null)
    {
      var createLevel = new CreateLevelWindow();
      createLevel.Owner = owner;
      createLevel.Show();
    }

    public static void OpenLevel(LevelDescription level, string file)
    {
      var editor = new Windows.LevelEditor();
      editor.LoadLevel(level, file);
      editor.Show();
    }

    public static bool PlayLevel(string file)
    {
      try
      {
        var level = LoadLevel(file);
        if (level != null)
        {
          PlayLevel(level);
          return true;
        }
      }
      catch (Exception e)
      {
        MessageBox.Show("error: " + e.Message);
      }
      return false;
    }

    public static LevelDescription LoadLevel(string file)
    {
      try
      {
        var level = JsonConvert.DeserializeObject<LevelDescription>(File.ReadAllText(file));
        if (level.Tiles == null)
        {
          level = LevelConverter.LevelConverter.Convert(AppSettings.Setting, JsonConvert.DeserializeObject<MCLevel>(File.ReadAllText(file)));
          if (level.Tiles == null) throw new FileFormatException();
        }
        return level;
      }
      catch (Exception e)
      {
        MessageBox.Show("parse error: " + e.Message);
        return null;
      }
    }

    public static void PlayLevel(LevelDescription level, int energy = 1000)
    {
      var player = new LevelPlayer();
      player.Loaded += (sender, args) =>
      {
        player.LoadLevel(AppSettings.Setting, level, energy);
      };
      player.Show();
    }

    public static bool OpenLevel(string file)
    {
      try
      {
        var level = LoadLevel(file);
        if (level != null)
        {
          Exception exc;
          if (!LevelDescriptionTestUtils.Test(level, out exc))
          {
            MessageBox.Show($"level not valid, exception: {exc.Message}");
          }
          else
          {
            OpenLevel(level, file);
            return true;
          }
        }
      }
      catch (Exception e)
      {
        MessageBox.Show($"parse error, exception: {e.Message}");
      }
      return false;
    }
  }
}
