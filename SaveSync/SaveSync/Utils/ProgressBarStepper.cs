using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveSync.Utils
{
  public class ProgressBarStepper
  {
    private float step;
    private float currentValue;

    public int Value => (int)currentValue;

    public ProgressBarStepper(int itemsCount)
    {
      step = 100f / itemsCount;
    }

    public int Step()
    {
      currentValue += step;
      return Value;
    }
    
  }
}
