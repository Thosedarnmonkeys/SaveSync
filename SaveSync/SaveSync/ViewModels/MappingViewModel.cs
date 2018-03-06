using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using SaveSync.Mapping;
using SaveSync.Utils;

namespace SaveSync.ViewModels
{
  public class MappingViewModel : DependencyObject
  {
    public FolderMapping Mapping { get; set; }

    #region ServerAge
    private DateTime serverAge;
    public DateTime ServerAge
    {
      get { return serverAge; }
      set
      {
        serverAge = value;
        LocalNewer = ClientAge > ServerAge;
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
        DependencyProperty.Register("LocalNewer", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false)); 
    #endregion

    public string ServerAgeString
    {
      get
      {
        if (ServerAge == DateTime.MinValue)
          return "-";

        return ServerAge.ToString();
      }
    }

    public string ClientAgeString
    {
      get
      {
        if (ClientAge == DateTime.MinValue)
          return "-";

        return ClientAge.ToString();
      }
    }

    public DelegateCommand BrowseFolderCommand { get; private set; }

    public MappingViewModel()
    {
      BrowseFolderCommand = new DelegateCommand(BrowseForFolder);
      Mapping = new FolderMapping();
    }

    private void BrowseForFolder()
    {
      using (var fbd = new FolderBrowserDialog())
      {
        DialogResult result = fbd.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
          Mapping.ClientSidePath = fbd.SelectedPath;
      }
    }
  }
}
