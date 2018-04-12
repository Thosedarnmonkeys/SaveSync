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
		private FolderMapping mapping;
    public FolderMapping Mapping
		{
			get { return mapping; }
			set
			{
				mapping = value;
				if (mapping == null)
					return;

				LocalFolderPath = mapping.ClientSidePath;
				FriendlyName = mapping.FriendlyName;
			}
		}

    public bool IsNewMapping { get; set; }

		#region ServerAge
		public DateTime ServerAge
		{
			get { return (DateTime)GetValue(ServerAgeProperty); }
			set { SetValue(ServerAgeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ServerAge.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ServerAgeProperty =
				DependencyProperty.Register("ServerAge", typeof(DateTime), typeof(MappingViewModel), new PropertyMetadata(DateTime.MinValue, SetServerAgeCallback));
		#endregion

		#region ClientAge
		public DateTime ClientAge
		{
			get { return (DateTime)GetValue(ClientAgeProperty); }
			set { SetValue(ClientAgeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ClientAge.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ClientAgeProperty =
				DependencyProperty.Register("ClientAge", typeof(DateTime), typeof(MappingViewModel), new PropertyMetadata(DateTime.MinValue, SetClientAgeCallback));
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

		#region LocalFolderPath
		public string LocalFolderPath
		{
			get { return (string)GetValue(LocalFolderPathProperty); }
			set { SetValue(LocalFolderPathProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LocalFolderPath.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LocalFolderPathProperty =
				DependencyProperty.Register("LocalFolderPath", typeof(string), typeof(MappingViewModel), new PropertyMetadata(null, SetMappingLocalFolderPathCallback));

		#endregion

		#region FriendlyName
		public string FriendlyName
		{
			get { return (string)GetValue(FriendlyNameProperty); }
			set { SetValue(FriendlyNameProperty, value); }
		}

		// Using a DependencyProperty as the backing store for FriendlyName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FriendlyNameProperty =
				DependencyProperty.Register("FriendlyName", typeof(string), typeof(MappingViewModel), new PropertyMetadata(null, SetMappingFriendlyNameCallback));
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

		public async void UpdateLastSyncTime()
		{
			ServerAge = await connection.LatestSync(mapping);
		}
    #endregion

    #region private methods
    private void BrowseForFolder()
    {
      using (var fbd = new FolderBrowserDialog())
      {
        DialogResult result = fbd.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
          LocalFolderPath = fbd.SelectedPath;
      }
    }

    private async Task DownloadFolder()
    {
      await connection.DownloadFolder(Mapping);
      await UpdateLocalAndServerAge();
    }

    private async Task UploadFolder()
    {
      await connection.UploadFolder(Mapping);
      await UpdateLocalAndServerAge();
    }

    private bool CanRunConnectionTasks()
    {
      return connection != null;
    }

    private async Task UpdateLocalAndServerAge()
    {
      ClientAge = DirUtils.GetLatestFileWriteTimeInDir(mapping.ClientSidePath);
      ServerAge = await connection.LatestSync(mapping);
    }

    #endregion

    #region callbacks
    private static void SetMappingLocalFolderPathCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o is MappingViewModel vm)
			{
				vm.Mapping.ClientSidePath = (string)e.NewValue;
			}
		}

		private static void SetMappingFriendlyNameCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o is MappingViewModel vm)
			{
				vm.Mapping.FriendlyName = (string)e.NewValue;
			}
		}

		private static void SetServerAgeCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o is MappingViewModel vm)
			{
				vm.ServerAge = (DateTime)e.NewValue;
				vm.LocalNewer = vm.ClientAge > vm.ServerAge;
				vm.ServerAgeString = vm.ServerAge == DateTime.MinValue ? "No Data" : vm.ServerAge.ToString();
			}
		}

		private static void SetClientAgeCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o is MappingViewModel vm)
			{
				vm.ClientAge = (DateTime)e.NewValue;
				vm.LocalNewer = vm.ClientAge > vm.ServerAge;
				vm.ClientAgeString = vm.ClientAge == DateTime.MinValue ? "No Data" : vm.ClientAge.ToString();
			}
		}
		#endregion
	}
}
