﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SaveSync.Config;
using SaveSync.Mapping;
using SaveSync.ServerConnection;

namespace SaveSync.ViewModels
{
  public class MainViewModel : DependencyObject
  {
    #region private fields

    private IServerConnection serverConnection;
    #endregion

    #region public properties
    #region Hostname
    public string Hostname
    {
      get { return (string)GetValue(HostnameProperty); }
      set { SetValue(HostnameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Hostname.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HostnameProperty =
        DependencyProperty.Register("Hostname", typeof(string), typeof(MainViewModel), new PropertyMetadata(null));
    #endregion

    #region Username
    public string Username
    {
      get { return (string)GetValue(UsernameProperty); }
      set { SetValue(UsernameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Username.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UsernameProperty =
        DependencyProperty.Register("Username", typeof(string), typeof(MainViewModel), new PropertyMetadata(null));
    #endregion

    #region Operation Progress
    public int OperationProgress
    {
      get { return (int)GetValue(OperationProgressProperty); }
      set { SetValue(OperationProgressProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OperationProgress.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OperationProgressProperty =
        DependencyProperty.Register("OperationProgress", typeof(int), typeof(MainViewModel), new PropertyMetadata(0));
    #endregion

    #region File Root
    public string FileRoot
    {
      get { return (string)GetValue(FileRootProperty); }
      set { SetValue(FileRootProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FileRoot.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FileRootProperty =
        DependencyProperty.Register("FileRoot", typeof(string), typeof(MainViewModel), new PropertyMetadata(null));
    #endregion

    #region Connection Type
    public ConnectionType ConnectionType
    {
      get { return (ConnectionType)GetValue(ConnectionTypeProperty); }
      set { SetValue(ConnectionTypeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ConnectionType.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ConnectionTypeProperty =
        DependencyProperty.Register("ConnectionType", typeof(ConnectionType), typeof(MainViewModel), new PropertyMetadata(ConnectionType.Ftp));
    #endregion

    #region Connected
    public bool Connected
    {
      get { return (bool)GetValue(ConnectedProperty); }
      set { SetValue(ConnectedProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Connected.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ConnectedProperty =
        DependencyProperty.Register("Connected", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
    #endregion

    public List<MappingViewModel> Mappings { get; set; }
    #endregion

    #region constructor

    public MainViewModel(SyncConfig config)
    {
      Hostname = config.Hostname;
      Username = config.Username;
      FileRoot = config.FileRoot;
      ConnectionType = config.ConnectionType;

      List<FolderMapping> mappings = config.Mappings;
      PopulateMappingServerPath(mappings, FileRoot);
      Mappings = CreateMappingVms(mappings);

      if (!string.IsNullOrWhiteSpace(Hostname)
          && !string.IsNullOrWhiteSpace(Username)
          && !string.IsNullOrWhiteSpace(FileRoot))
      {
        serverConnection = CreateServerConnection(ConnectionType);
        Connected = serverConnection.TestConnection();
      }
    }
    #endregion

    #region private methods
    private List<MappingViewModel> CreateMappingVms(List<FolderMapping> mappings)
    {
      var results = new List<MappingViewModel>();

      if (mappings == null)
        return results;

      foreach (FolderMapping mapping in mappings)
      {
        var vm = new MappingViewModel();
        vm.Mapping = mapping;

        if (Connected)
          vm.ServerAge = serverConnection.LatestSync(mapping);

        results.Add(vm);
      }

      return results;
    }

    private IServerConnection CreateServerConnection(ConnectionType connectionType)
    {
      switch (connectionType)
      {
        case ConnectionType.Ftp:
          return new FtpServerConnection(Hostname, Username, FileRoot);
        case ConnectionType.Http:
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void PopulateMappingServerPath(List<FolderMapping> mappings, string serverFolderRoot)
    {
      if (mappings == null || serverFolderRoot == null)
        return;

      foreach (FolderMapping mapping in mappings)
      {
        mapping.ServerSidePath = serverFolderRoot + Path.DirectorySeparatorChar + mapping.FriendlyName;
      }
    }

    private void UploadFolder(FolderMapping mapping)
    {
      if (mapping == null || serverConnection == null)
        return;

      serverConnection.UploadFolder(mapping);
    }
    #endregion
  }
}
