using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using Tectil.NCommand.Contract;

namespace Match3Tools.Commands
{
  public class DigitFileRenameCommand
  {
    public enum Operator
    {
      Plus,
      Minus
    }

    public class FileData
    {
      public string Path;
      public string NewPath;
      public byte[] Bytes;
    }

    [Command("digitFileRename")]
    public void DigitFileRename([Argument("path")]string path, [Argument("filter", "*")]string filter, [Argument("operator", "+", Description = "+|-")]string operatorType, [Argument("value", 0)]int value)
    {
      Contract.Assert(operatorType == "+" || operatorType == "-");

      Operator type = Operator.Plus;
      if (operatorType == "+") type = Operator.Plus;
      else if (operatorType == "-") type = Operator.Minus;

      if (File.Exists(path))
      {
        Operate(path, File.ReadAllBytes(path), type, value);
      }
      else if (Directory.Exists(path))
      {

        var files = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
        var datas = new List<FileData>(files.Length);
        foreach (var file in files)
        {
          if (File.Exists(file))
          {
            datas.Add(new FileData
            {
              Path = file,
              Bytes = File.ReadAllBytes(file)
            });
          }
        }

        foreach (var fileData in datas)
        {
          fileData.NewPath = Operate(fileData.Path, fileData.Bytes, type, value);
        }

        foreach (var fileData in datas)
        {
          File.Delete(fileData.Path);
        }

        foreach (var fileData in datas)
        {
          File.WriteAllBytes(fileData.NewPath, fileData.Bytes);
        }
      }
    }

    private string Operate(string file, byte[] bytes, Operator operatorType, int value)
    {
      var fileName = Path.GetFileNameWithoutExtension(file);
      var fileExtension = Path.GetExtension(file);
      var fileDir = Path.GetDirectoryName(file);

      var newName = new StringBuilder();
      foreach (var c in fileName)
      {
        if (char.IsDigit(c))
        {
          newName.Append(c);
        }
      }
      Contract.Assert(newName.Length != 0);
      var nameInt = int.Parse(newName.ToString());
      var result = nameInt;
      switch (operatorType)
      {
        case Operator.Plus:
          result = nameInt + value;
          break;
        case Operator.Minus:
          result = nameInt - value;
          break;
      }

      var path = Directory.Exists(fileDir) ? Path.Combine(fileDir, result.ToString()) : result.ToString();
      path = Path.ChangeExtension(path, fileExtension);
      return path;
    }
  }
}
