using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Match3.Editor
{
  public class RecentlyFiles : INotifyPropertyChanged
  {
    private static RecentlyFiles _instance;
    public static RecentlyFiles Instance
    {
      get { return _instance ?? (_instance = new RecentlyFiles()); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [Bindable(true)]
    public string Workspace;
    [Bindable(true)]
    public List<RecentlyFile> RecentlyOpened = new List<RecentlyFile>();

    public void AddRecently(string file)
    {
      RecentlyFile recentlyFile;
      if ((recentlyFile = RecentlyOpened.FirstOrDefault(f => f.Path == file)) == null)
      {
        if (File.Exists(file))
        {
          RecentlyOpened.Add(new RecentlyFile
          {
            Path = file,
            Time = DateTime.Now
          });
          var sorted = RecentlyOpened.OrderByDescending(r => r.Time).ToList();
          RecentlyOpened.Clear();
          RecentlyOpened.AddRange(sorted);
          SaveRecently();

          if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(nameof(RecentlyOpened)));
        }
      }
      else
      {
        recentlyFile.Time = DateTime.Now;
        var sorted = RecentlyOpened.OrderByDescending(r => r.Time).ToList();
        RecentlyOpened.Clear();
        RecentlyOpened.AddRange(sorted);

        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(nameof(RecentlyOpened)));
      }
    }

    public void RemoveRecently(RecentlyFile file)
    {
      RecentlyOpened.Remove(file);
      if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(nameof(RecentlyOpened)));
    }

    public void LoadRecently()
    {
      var directory = Environment.CurrentDirectory;
      var file = System.IO.Path.Combine(directory, "recently.json");
      if (File.Exists(file))
      {
        var json = JsonSerializer.CreateDefault().Deserialize<Recently>(new JsonTextReader(new StreamReader(file)));
        if (json != null)
        {
          Workspace = json.Worksapce;
          RecentlyOpened.AddRange(json.Files);
          if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(nameof(RecentlyOpened)));
          if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(nameof(Workspace)));
        }
      }
    }

    public void SaveRecently()
    {
      var directory = Environment.CurrentDirectory;
      var file = System.IO.Path.Combine(directory, "recently.json");
      var json = JsonConvert.SerializeObject(new Recently
      {
        Worksapce = Workspace,
        Files = RecentlyOpened.ToArray()
      });
      File.WriteAllText(file, json);
    }

    public class Recently
    {
      public RecentlyFile[] Files;
      public string Worksapce;
    }

    public class RecentlyFile : IComparable<RecentlyFile>
    {
      public string Path;
      public DateTime Time;

      public int CompareTo(RecentlyFile other)
      {
        return Time > other.Time ? 1 : -1;
      }
    }
  }
}
