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
    Task<bool> TestConnection();
    Task<DateTime> LatestSync(FolderMapping mapping);
    Task UploadFolder(FolderMapping mapping);
    Task UploadFolders(List<FolderMapping> mappings);
    Task DownloadFolder(FolderMapping mapping);
    Task DownloadFolders(List<FolderMapping> mappings);
    Task CloseConnection();
  }
}
