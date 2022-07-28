using System;
using Tectil.NCommand;

namespace Match3Tools
{
  class Program
  {
    static void Main(string[] args)
    {
      NCommands commands = new NCommands();
      commands.RunConsole(args);
    }
  }
}
