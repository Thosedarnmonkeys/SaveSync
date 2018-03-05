using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SaveSync.Config;
using SaveSync.ViewModels;

namespace SaveSync
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      SyncConfig config = ConfigManager.ReadConfig();
      DataContext = new MainViewModel(config);
      Closing += WriteConfig;
    }

    private void WriteConfig(object o, EventArgs e)
    {
      ((MainViewModel) DataContext).WriteConfig();
    }

    private void FtpPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
      ((MainViewModel) DataContext).FtpPassword = FtpPasswordBox.Password;
    }
  }
}
