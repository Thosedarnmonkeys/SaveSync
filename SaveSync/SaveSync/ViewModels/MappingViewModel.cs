using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaveSync.Mapping;

namespace SaveSync.ViewModels
{
  public class MappingViewModel
  {
    public FolderMapping Mapping { get; set; }
    public DateTime ServerAge { get; set; }
    public DateTime ClientAge { get; set; }
  }
}
