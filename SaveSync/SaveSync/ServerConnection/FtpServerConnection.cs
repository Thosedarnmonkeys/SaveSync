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

      List<DateTime> folderDateTimes = folders.Select(x => DateTimeDirUtils.GetDirDateTime(x.Name)).ToList();
      folderDateTimes.Sort();
      return folderDateTimes.Last();
    }

    public async Task UploadFolder(FolderMapping mapping)
    {
      progressUpdateAction.Invoke(0);

      var stepper = new ProgressBarStepper(GetLocalFiles(mapping.ClientSidePath).Length);
      await UploadFolderImpl(mapping, stepper);

      progressUpdateAction.Invoke(100);
    }

    public async Task UploadFolders(List<FolderMapping> mappings, ProgressBarStepper stepper)
    {
      foreach (FolderMapping mapping in mappings)
      {
        await UploadFolderImpl(mapping, stepper);
      }
    }

    public async Task DownloadFolder(FolderMapping mapping)
    {
      progressUpdateAction.Invoke(0); 

      var stepper = new ProgressBarStepper(await GetRemoteFilesCount(mapping));
      await DownloadFolderImpl(mapping, stepper);

      progressUpdateAction.Invoke(100);
    }

    public async Task DownloadFolders(List<FolderMapping> mappings, ProgressBarStepper stepper)
    {
      foreach (FolderMapping mapping in mappings)
      {
        await DownloadFolderImpl(mapping, stepper);
      }
    }

    public async Task<int> FileCount(List<FolderMapping> mappings)
    {
      string[] dirs = mappings.Select(x => x.FriendlyName).ToArray();
      return await GetRemoteFilesCount(dirs);
    }


    public async Task CloseConnection()
    {
      await client.DisconnectAsync();
    }

    #endregion

    #region private methods

    private async Task<DateTime> LatestSync(string friendlyName)
    {
      string path = folderRoot + friendlyName + "/";
      if (!await client.DirectoryExistsAsync(path))
        return DateTime.MinValue;

      FtpListItem[] folders = await client.GetListingAsync(path);
      if (folders.Length == 0)
        return DateTime.MinValue;

      List<DateTime> folderDateTimes = folders.Select(x => DateTimeDirUtils.GetDirDateTime(x.Name)).ToList();
      folderDateTimes.Sort();
      return folderDateTimes.Last();
    }

    private async Task UploadFolderImpl(FolderMapping mapping, ProgressBarStepper stepper)
    {
      string[] files = GetLocalFiles(mapping.ClientSidePath);
      var filePaths = from f in files
                      select f.Substring(mapping.ClientSidePath.Length);
      DateTime uploadDateTime = DirUtils.GetLatestFileWriteTimeInDir(mapping.ClientSidePath);
      string datetimeFolderString = DateTimeDirUtils.GetDirDateTimeString(uploadDateTime);
      string serverSideFolderPath = folderRoot + mapping.FriendlyName + "/" + datetimeFolderString;

      client.CreateDirectory(serverSideFolderPath);

      foreach (string file in filePaths)
      {
        string uploadPath = serverSideFolderPath + file.Replace(@"\", "/");
        await client.UploadFileAsync(mapping.ClientSidePath + file, uploadPath, FtpExists.Overwrite, true, FtpVerify.Retry);
        progressUpdateAction.Invoke(stepper.Step());
      }
    }

    private async Task DownloadFolderImpl(FolderMapping mapping, ProgressBarStepper stepper)
    {
      DateTime latestSync = await LatestSync(mapping);
      string remotePath = folderRoot + mapping.FriendlyName + "/" + DateTimeDirUtils.GetDirDateTimeString(latestSync) + "/";

      List<FtpListItem> remoteFiles = await client.GetRecursiveListing(remotePath);
      foreach (FtpListItem file in remoteFiles)
      {
        string clientSidePath = mapping.ClientSidePath + @"\" + file.FullName.Substring(remotePath.Length).Replace("/", @"\");
        await client.DownloadFileAsync(clientSidePath, file.FullName, true, FtpVerify.Retry);
        File.SetLastWriteTime(clientSidePath, latestSync);
        progressUpdateAction.Invoke(stepper.Step());
      }
    }

    private string[] GetLocalFiles(string clientSidePath)
    {
      return Directory.GetFiles(clientSidePath, "*", SearchOption.AllDirectories);
    }

    private async Task<int> GetRemoteFilesCount(string[] friendlyNames)
    {
      for (int i = 0; i < friendlyNames.Length; i++)
      {
        DateTime latestSync = await LatestSync(friendlyNames[i]);
        friendlyNames[i] = folderRoot + friendlyNames[i] + "/" + DateTimeDirUtils.GetDirDateTimeString(latestSync) + "/";
      }

      List<FtpListItem> files = await client.GetRecursiveListing(friendlyNames);
      return files.Count;
    }

    private async Task<int> GetRemoteFilesCount(FolderMapping mapping)
    {
      DateTime latestSync = await LatestSync(mapping);
      string remotePath = folderRoot + mapping.FriendlyName + "/" + DateTimeDirUtils.GetDirDateTimeString(latestSync) + "/";

      List<FtpListItem> files = await client.GetRecursiveListing(remotePath);
      return files.Count;
    }
    #endregion
  }

  #region extension method class
  public static class FlientFtpClientExtensionMethods
  {
    public static async Task<List<FtpListItem>> GetRecursiveListing(this FtpClient ftpClient, params string[] dirs)
    {
      var items = new List<FtpListItem>();

      foreach (string dir in dirs)
      {
        foreach (FtpListItem item in ftpClient.GetListing(dir))
        {
          switch (item.Type)
          {
            case FtpFileSystemObjectType.File:
              items.Add(item);
              break;
            case FtpFileSystemObjectType.Directory:
              items.AddRange(await GetRecursiveListing(ftpClient, item.FullName));
              break;
            case FtpFileSystemObjectType.Link:
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }

      return items;
    }
  } 
  #endregion
}
