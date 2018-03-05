using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SaveSync.Mapping;
using SaveSync.ServerConnection;

namespace SaveSync.Config
{
  public class SyncConfig
  {
    [XmlAttribute]
    public string Hostname { get; set; }
    [XmlAttribute]
    public string Username { get; set; }
    [XmlAttribute]
    public string FileRoot { get; set; }
    [XmlAttribute]
    public ConnectionType ConnectionType { get; set; }
    [XmlAttribute]
    public string FtpUsername { get; set; }
    public List<FolderMapping> Mappings { get; set; }

    public override bool Equals(object o)
    {
      SyncConfig other = o as SyncConfig;
      if (other == null)
        return false;

      if (Hostname != other.Hostname)
        return false;

      if (Username != other.Username)
        return false;

      if (FileRoot != other.FileRoot)
        return false;

      if (ConnectionType != other.ConnectionType)
        return false;

      if (FtpUsername != other.FtpUsername)
        return false;

      if (Mappings != null || other.Mappings != null)
      {
        if (Mappings == null || other.Mappings == null)
          return false;

        var mymaps = new List<FolderMapping>(Mappings);
        mymaps.Sort();
        var othermaps = new List<FolderMapping>(other.Mappings);
        othermaps.Sort();
        
        if (!mymaps.SequenceEqual(othermaps, new FolderMappingEqualityComparer()))
          return false;
      }

      return true;
    }

    private class FolderMappingEqualityComparer : IEqualityComparer<FolderMapping>
    {
      public bool Equals(FolderMapping x, FolderMapping y)
      {
        if (x == null && y == null)
          return true;

        if (x == null || y == null)
          return false;

        if (x.FriendlyName != y.FriendlyName)
          return false;

        if (x.ClientSidePath != y.ClientSidePath)
          return false;

        return true;
      }

      public int GetHashCode(FolderMapping obj)
      {
        int hashcode = 1;

        hashcode *= obj.FriendlyName?.GetHashCode() ?? 1;
        hashcode *= obj.ClientSidePath?.GetHashCode() ?? 1;

        return hashcode;
      }
    }
  }
}
