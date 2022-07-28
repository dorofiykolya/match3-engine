using System.IO;

namespace Match3.Editor.LevelEditor
{
  public class LevelWorkspace
  {
    public string Directory;
    public string FileName;

    public LevelWorkspace()
    {

    }

    public LevelWorkspace(string file)
    {
      Directory = Path.GetDirectoryName(file);
      FileName = Path.GetFileName(file);
    }

    public string FilePath
    {
      get
      {
        return Path.Combine(Directory, FileName);
      }
    }
  }
}
