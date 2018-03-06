using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveSync.Utils
{
  public static class DirUtils
  {
    public static DateTime GetLatestFileWriteTimeInDir(string path)
    {
      if (path == null)
        return DateTime.MinValue;

      if (!Directory.Exists(path))
        return DateTime.MinValue;

      DateTime latest = DateTime.MinValue;
      string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
      foreach (string file in files)
      {
        DateTime lastWrite = File.GetLastWriteTime(file);
        if (lastWrite > latest)
          latest = lastWrite;
      }

      return latest;
    }
  }
}
