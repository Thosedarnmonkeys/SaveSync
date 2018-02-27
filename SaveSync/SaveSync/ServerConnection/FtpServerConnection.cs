using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaveSync.Mapping;

namespace SaveSync.ServerConnection
{
  public class FtpServerConnection : IServerConnection
  {
    #region private fields
    private string hostname;
    private string username;
    private string fileRoot;
    #endregion


    #region constructor
    public FtpServerConnection(string hostname, string username, string fileRoot)
    {
      this.hostname = hostname;
      this.username = username;
      this.fileRoot = fileRoot;
    }
    #endregion

    #region public methods
    public bool TestConnection()
    {
      throw new NotImplementedException();
    }

    public DateTime LatestSync(FolderMapping mapping)
    {
      throw new NotImplementedException();
    }

    public void UploadFolder(FolderMapping mapping)
    {
      throw new NotImplementedException();
    }

    public void UploadFolders(List<FolderMapping> mappings)
    {
      throw new NotImplementedException();
    }

    public void DownloadFolder(FolderMapping mapping)
    {
      throw new NotImplementedException();
    }

    public void DownloadFolders(List<FolderMapping> mappings)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
