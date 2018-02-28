using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaveSync.Config;
using SaveSync.Mapping;
using SaveSync.ServerConnection;

namespace SaveSyncTests
{
  [TestClass]
  public class ConfigManagerTests
  {
    [TestMethod]
    public void TestWrite()
    {
      var config = new SyncConfig()
      {
        Hostname = "a server",
        FileRoot = "a root",
        Username = "a user",
        ConnectionType = ConnectionType.Http,
        FtpUsername = "ftper",
        FtpPassword = "ftpwd",
        Mappings = new List<FolderMapping>()
        {
          new FolderMapping(){FriendlyName = "A folder", ClientSidePath = "a path"},
          new FolderMapping(){FriendlyName = "Another folder", ClientSidePath = "another path"},
          new FolderMapping(){FriendlyName = "A third folder", ClientSidePath = "a third path"},
        }
      };

      ConfigManager.WriteConfig(config);

      Assert.AreEqual(GetFileText("SaveSyncTests.Resources.TestWrite.txt"), GetTestFileText());
    }

    [TestMethod]
    public void TestRead()
    {
      var config = new SyncConfig()
      {
        Hostname = "a server",
        FileRoot = "a root",
        Username = "a user",
        ConnectionType = ConnectionType.Http,
        FtpUsername = "ftper",
        FtpPassword = "ftpwd",
        Mappings = new List<FolderMapping>()
        {
          new FolderMapping(){FriendlyName = "A folder", ClientSidePath = "a path"},
          new FolderMapping(){FriendlyName = "Another folder", ClientSidePath = "another path"},
          new FolderMapping(){FriendlyName = "A third folder", ClientSidePath = "a third path"},
        }
      };

      Assert.AreEqual(config, ConfigManager.ReadConfig());
    }

    private string GetFileText(string resourceName)
    {
      using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)))
      {
        return reader.ReadToEnd();
      }
    }

    private string GetTestFileText()
    {
      return File.ReadAllText(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "savesync.conf");
    }
  }
}
