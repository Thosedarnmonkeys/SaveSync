using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaveSync.Config;
using SaveSync.ServerConnection;

namespace SaveSyncTests.Tests
{
  [TestClass]
  public class MainViewModelTests
  {
    [TestMethod]
    public void TestBuildWithValidConf()
    {
      SyncConfig conf = new SyncConfig()
      {
        ConnectionType = ConnectionType.Ftp,
        FileRoot = "E:\\"
      };
    }
  }
}
