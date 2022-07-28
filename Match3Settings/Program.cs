using System;
using System.IO;
using Match3.Settings;

namespace Match3Settings
{
  class Program
  {
    static void Main(string[] args)
    {
      var settings = Match3SettingsParser.Parse(File.ReadAllBytes("Match3Settings.xml"));
      Console.ReadKey();
    }
  }
}
