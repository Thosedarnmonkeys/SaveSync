using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using SaveSync.Mapping;
using SaveSync.ServerConnection;
using SaveSync.Utils;

namespace SaveSync.ViewModels
{
  public class MappingViewModel : DependencyObject
  {
    #region private fields
    private IServerConnection connection;
    #endregion

    #region public properties
    public FolderMapping Mapping { get; set; }

    public bool IsNewMapping { get; set; }

    #region ServerAge
    private DateTime serverAge;
    public DateTime ServerAge
    {
      get { return serverAge; }
      set
      {
        serverAge = value;
        LocalNewer = ClientAge > ServerAge;
        ServerAgeString = serverAge == DateTime.MinValue ? "No Data" : serverAge.ToString();
      }
    }
    #endregion

    #region ClientAge
    private DateTime clientAge;
    public DateTime ClientAge
    {
      get { return clientAge; }
      set
      {
        clientAge = value;
        LocalNewer = ClientAge > ServerAge;
        ClientAgeString = clientAge == DateTime.MinValue ? "No Data" : clientAge.ToString();
      }
    }
    #endregion

    #region LocalNewer
    public bool LocalNewer
    {
      get { return (bool)GetValue(LocalNewerProperty); }
      set { SetValue(LocalNewerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LocalNewer.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LocalNewerProperty =
        DependencyProperty.Register("LocalNewer", typeof(bool), typeof(MappingViewModel), new PropertyMetadata(false));
    #endregion

    #region ServerAgeString
    public string ServerAgeString
    {
      get { return (string)GetValue(ServerAgeStringProperty); }
      set { SetValue(ServerAgeStringProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ServerAgeString.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ServerAgeStringProperty =
        DependencyProperty.Register("ServerAgeString", typeof(string), typeof(MappingViewModel), new PropertyMetadata(null));
    #endregion

    #region ClientAgeString
    public string ClientAgeString
    {
      get { return (string)GetValue(ClientAgeStringProperty); }
      set { SetValue(ClientAgeStringProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ClientAgeString.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ClientAgeStringProperty =
        DependencyProperty.Register("ClientAgeString", typeof(string), typeof(MappingViewModel), new PropertyMetadata(null));
    #endregion

    public DelegateCommand BrowseFolderCommand { get; private set; }
    public AsyncDelegateCommand DownloadFolderCommand { get; private set; }
    public AsyncDelegateCommand UploadFolderCommand { get; private set; }
    #endregion

    #region constructor
    public MappingViewModel()
    {
      Mapping = new FolderMapping();
      BrowseFolderCommand = new DelegateCommand(BrowseForFolder);
      DownloadFolderCommand = new AsyncDelegateCommand(DownloadFolder, CanRunConnectionTasks);
      UploadFolderCommand = new AsyncDelegateCommand(UploadFolder, CanRunConnectionTasks);
    }

    #endregion

    #region public methods

    public void AddConnection(IServerConnection connection)
    {
      this.connection = connection;
    }
    #endregion

    #region private methods
    private void BrowseForFolder()
    {
      using (var fbd = new FolderBrowserDialog())
      {
        DialogResult result = fbd.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
          Mapping.ClientSidePath = fbd.SelectedPath;
      }
    }

    private async Task DownloadFolder()
    {
      await connection.DownloadFolder(Mapping);
    }

    private async Task UploadFolder()
    {
      await connection.UploadFolder(Mapping);
    }

    private bool CanRunConnectionTasks()
    {
      return connection != null;
    }
    #endregion
  }
}
