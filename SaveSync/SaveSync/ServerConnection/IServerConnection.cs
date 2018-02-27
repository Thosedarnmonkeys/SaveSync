using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaveSync.Mapping;

namespace SaveSync.ServerConnection
{
  public interface IServerConnection
  {
    bool TestConnection();
    DateTime LatestSync(FolderMapping mapping);
    void UploadFolder(FolderMapping mapping);
    void UploadFolders(List<FolderMapping> mappings);
    void DownloadFolder(FolderMapping mapping);
    void DownloadFolders(List<FolderMapping> mappings);
  }
}
