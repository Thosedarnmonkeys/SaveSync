using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SaveSync.Mapping
{
  public class FolderMapping
  {
    public string FriendlyName { get; set; }
    public string ClientSidePath { get; set; }
  }
}
