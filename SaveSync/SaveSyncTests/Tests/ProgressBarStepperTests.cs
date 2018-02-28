using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaveSync.Utils;

namespace SaveSyncTests.Tests
{
  [TestClass]
  public class ProgressBarStepperTests
  {
    [TestMethod]
    public void Test100Items()
    {
      var stepper = new ProgressBarStepper(100);
      Assert.AreEqual(0, stepper.Value);

      for (int i = 0; i < 101; i++)
      {
        Assert.AreEqual(i, stepper.Value);
        Assert.AreEqual(i + 1, stepper.Step());
      }
    }

    [TestMethod]
    public void Test101Items()
    {
      var stepper = new ProgressBarStepper(101);
      Assert.AreEqual(0, stepper.Value);
      Assert.AreEqual(0, stepper.Step());
      Assert.AreEqual(1, stepper.Step());

      for (int i = 0; i < 49; i++)
        stepper.Step();

      Assert.AreEqual(50, stepper.Value);
      Assert.AreEqual(51, stepper.Step());

      for (int i = 0; i < 49; i++)
        stepper.Step();

      Assert.AreEqual(99, stepper.Value);
      Assert.AreEqual(100, stepper.Step());
    }
  }
}
