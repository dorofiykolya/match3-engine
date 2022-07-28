using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3ViewTest
{
  public class ConsolePaint
  {
    public static void StartConsolePaint()
    {
      Console.CursorSize = 99;
      Console.Write(" ");
      short sh1 = 0, sh2 = 0;
      bool flag1 = false, flag2 = false;
      Console.SetCursorPosition(0, 21);
      Console.Write("********************************************************************************");
      Console.SetCursorPosition(0, 22);
      Console.Write("*  Colors: (W)hite , (R)ed , (G)reen , (B)lue , (C)yan , (Y)ellow , (M)agenta  *");
      Console.SetCursorPosition(0, 23);
      Console.Write("*  Delete(black): X    <     Dark/LightColors: O     >    Draw(On/Off): Enter  *");
      Console.SetCursorPosition(0, 24);
      Console.Write("********************************(c)Vicces2009***********************************");
      Console.SetCursorPosition(0, 0);
      Console.BackgroundColor = ConsoleColor.White;
      while (true)
      {
        ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
        bool flag3 = consoleKeyInfo.Key.ToString() != "DownArrow";
        if (!flag3)
        {
          flag3 = sh2 >= 20;
          if (!flag3)
            sh2 += (short)1;
        }
        else
        {
          flag3 = consoleKeyInfo.Key.ToString() != "UpArrow";
          if (!flag3)
          {
            flag3 = sh2 <= 0;
            if (!flag3)
              sh2 -= (short)1;
          }
          else
          {
            flag3 = consoleKeyInfo.Key.ToString() != "RightArrow";
            if (!flag3)
            {
              flag3 = sh1 >= 79;
              if (!flag3)
                sh1 += (short)1;
            }
          }
        }
        flag3 = consoleKeyInfo.Key.ToString() != "LeftArrow";
        if (!flag3)
        {
          flag3 = sh1 <= 0;
          if (!flag3)
            sh1 -= (short)1;
        }
        flag3 = consoleKeyInfo.Key.ToString() != "Enter";
        if (!flag3)
        {
          flag3 = !flag1;
          if (!flag3)
            flag1 = false;
          else
            flag1 = true;
        }
        else
        {
          flag3 = consoleKeyInfo.Key.ToString() != "O";
          if (!flag3)
          {
            flag3 = !flag2;
            if (!flag3)
              flag2 = false;
            else
              flag2 = true;
          }
          else
          {
            flag3 = consoleKeyInfo.Key.ToString() != "Escape";
            if (!flag3)
              break;
          }
          flag3 = flag2;
          if (!flag3)
          {
            flag3 = consoleKeyInfo.Key.ToString() != "W";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.White;
            flag3 = consoleKeyInfo.Key.ToString() != "R";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Red;
            flag3 = consoleKeyInfo.Key.ToString() != "G";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Green;
            flag3 = consoleKeyInfo.Key.ToString() != "B";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Blue;
            flag3 = consoleKeyInfo.Key.ToString() != "C";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Cyan;
            flag3 = consoleKeyInfo.Key.ToString() != "Y";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Yellow;
            flag3 = consoleKeyInfo.Key.ToString() != "M";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Magenta;
            flag3 = consoleKeyInfo.Key.ToString() != "X";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Black;
          }
          else
          {
            flag3 = consoleKeyInfo.Key.ToString() != "W";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Gray;
            flag3 = consoleKeyInfo.Key.ToString() != "R";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.DarkRed;
            flag3 = consoleKeyInfo.Key.ToString() != "G";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.DarkGreen;
            flag3 = consoleKeyInfo.Key.ToString() != "B";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.DarkBlue;
            flag3 = consoleKeyInfo.Key.ToString() != "C";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.DarkCyan;
            flag3 = consoleKeyInfo.Key.ToString() != "Y";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.DarkYellow;
            flag3 = consoleKeyInfo.Key.ToString() != "M";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.DarkMagenta;
            flag3 = consoleKeyInfo.Key.ToString() != "X";
            if (!flag3)
              Console.BackgroundColor = ConsoleColor.Black;
          }
        }
        Console.SetCursorPosition(sh1, sh2);
        flag3 = !flag1;
        if (!flag3)
        {
          Console.Write(" ");
          Console.SetCursorPosition(sh1, sh2);
        }
      }
      Console.BackgroundColor = ConsoleColor.Black;
      Console.Clear();
    }
  }
}
