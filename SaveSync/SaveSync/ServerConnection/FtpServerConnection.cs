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

    private string folderRoot => "/" + fileRoot + "/" + username + "/";

    #region constructor
    public FtpServerConnection(string hostname, string username, string fileRoot, string ftpUsername, string ftpPassword, Action<int> progressUpdateAction)
    {
      this.hostname = hostname;
      this.username = username;
      this.fileRoot = fileRoot.Trim(new[] { '/' });
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

    public async Task<DateTime> LatestSync(FolderMapping mapping)
    {
      if (mapping == null || mapping.FriendlyName == null)
        return DateTime.MinValue;

      string path = folderRoot + mapping.FriendlyName + "/";
      if (!await client.DirectoryExistsAsync(path))
        return DateTime.MinValue;

      FtpListItem[] folders = await client.GetListingAsync(path);
      if (folders.Length == 0) 
        return DateTime.MinValue;

      List<DateTime> folderDateTimes = folders.Select(x => DateTimeDirReader.GetDirDateTime(x.Name)).ToList();
      folderDateTimes.Sort();
      return folderDateTimes.Last();
    }

    public async Task UploadFolder(FolderMapping mapping)
    {
      progressUpdateAction.Invoke(0);

      string[] files = Directory.GetFiles(mapping.ClientSidePath, "*", SearchOption.AllDirectories);
      var stepper = new ProgressBarStepper(files.Length);
      foreach (string file in files)
      {
        string uploadPath = folderRoot + mapping.FriendlyName + Path.DirectorySeparatorChar + file;
        await client.UploadFileAsync(mapping.ClientSidePath, uploadPath, FtpExists.Overwrite, true, FtpVerify.Retry);
        progressUpdateAction.Invoke(stepper.Step());
      }

      progressUpdateAction.Invoke(100);
    }

    public async Task UploadFolders(List<FolderMapping> mappings)
    {
      throw new NotImplementedException();
    }

    public async Task DownloadFolder(FolderMapping mapping)
    {
      throw new NotImplementedException();
    }

    public async Task DownloadFolders(List<FolderMapping> mappings)
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
