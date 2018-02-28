using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SaveSync.Mapping
{
  public class FolderMapping : IComparable
  {
    [XmlAttribute]
    public string FriendlyName { get; set; }
    [XmlAttribute]
    public string ClientSidePath { get; set; }

    public override bool Equals(object o)
    {
      FolderMapping other = o as FolderMapping;
      if (other == null)
        return false;

      if (FriendlyName != other.FriendlyName)
        return false;

      if (ClientSidePath != other.ClientSidePath)
        return false;

      return true;
    }

    public int CompareTo(object obj)
    {
      FolderMapping other = obj as FolderMapping;
      if (other == null)
        return -1;

      int compare = 0;
      if (FriendlyName != null)
        compare = FriendlyName.CompareTo(other.FriendlyName);

      else if (other.FriendlyName != null)
        compare = other.FriendlyName.CompareTo(FriendlyName);

      if (compare != 0)
        return compare;

      if (ClientSidePath != null)
        compare = ClientSidePath.CompareTo(other.ClientSidePath);

      else if (other.ClientSidePath != null)
        compare = other.ClientSidePath.CompareTo(ClientSidePath);

      return compare;
    }
  }
}
