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
using System.Windows.Shapes;
using SaveSync.ViewModels;

namespace SaveSync.Views
{
  /// <summary>
  /// Interaction logic for EditMappingWindow.xaml
  /// </summary>
  public partial class EditMappingWindow : Window
  {
    public EditMappingWindow(MappingViewModel vm)
    {
      InitializeComponent();
      DataContext = vm;
    }
  }
}
