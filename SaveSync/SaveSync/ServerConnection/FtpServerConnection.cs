using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using SaveSync.Mapping;
using SaveSync.Utils;
using Exception = System.Exception;

namespace SaveSync.ServerConnection
{
  public class FtpServerConnection : IServerConnection
  {
    #region private fields
    private string hostname;
    private string username;
    private string fileRoot;
    private FtpClient client;
    private Action<int> progressUpdateAction;
    #endregion

    private string folderRoot => fileRoot + Path.PathSeparator + username + Path.PathSeparator;

    #region constructor
    public FtpServerConnection(string hostname, string username, string fileRoot, string ftpUsername, string ftpPassword, Action<int> progressUpdateAction)
    {
      this.hostname = hostname;
      this.username = username;
      this.fileRoot = fileRoot;
      this.progressUpdateAction = progressUpdateAction;
      client = new FtpClient(hostname);
      client.RetryAttempts = 3;
      client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
    }
    #endregion

    #region public methods
    public async Task<bool> TestConnection()
    {
      try
      {
        await client.ConnectAsync();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public Task<DateTime> LatestSync(FolderMapping mapping)
    {
     throw new NotImplementedException(); 
    }

    public async Task UploadFolder(FolderMapping mapping)
    {
      progressUpdateAction.Invoke(0);

      string[] files = Directory.GetFiles(mapping.ClientSidePath, "*.*", SearchOption.AllDirectories);
      var stepper = new ProgressBarStepper(files.Length);
      foreach (string file in files)
      {
        string uploadPath = folderRoot + mapping.FriendlyName + Path.PathSeparator + file;
        await client.UploadFileAsync(mapping.ClientSidePath, uploadPath, FtpExists.Overwrite, true, FtpVerify.Retry);
        progressUpdateAction.Invoke(stepper.Step());
      }

      progressUpdateAction.Invoke(100);
    }

    public Task UploadFolders(List<FolderMapping> mappings)
    {
      throw new NotImplementedException();
    }

    public Task DownloadFolder(FolderMapping mapping)
    {
      throw new NotImplementedException();
    }

    public Task DownloadFolders(List<FolderMapping> mappings)
    {
      throw new NotImplementedException();
    }

    public async Task CloseConnection()
    {
      await client.DisconnectAsync();
    }

    #endregion
  }
}
