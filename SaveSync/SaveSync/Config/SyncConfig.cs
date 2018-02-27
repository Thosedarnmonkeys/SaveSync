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
    public List<FolderMapping> Mappings { get; set; }
  }
}
