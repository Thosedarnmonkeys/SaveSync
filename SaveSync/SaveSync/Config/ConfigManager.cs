using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SaveSync.Config
{
  public static class ConfigManager
  {
    private const string configFileName = "savesync.conf";

    public static SyncConfig ReadConfig()
    {
      string confPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + configFileName;
      var serializer = new XmlSerializer(typeof(SyncConfig));
      using (StreamReader reader = File.OpenText(confPath))
      {
        return (SyncConfig)serializer.Deserialize(reader);
      }
    }

    public static void WriteConfig(SyncConfig config)
    {
      string confPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + configFileName;
      var serializer = new XmlSerializer(typeof(SyncConfig));
      using (StreamWriter writer = File.CreateText(confPath))
      {
        serializer.Serialize(writer, config);
      }
    }
  }
}
