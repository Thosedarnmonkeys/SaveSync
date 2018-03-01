using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SaveSync.Utils
{
  public class AsyncDelegateCommand : ICommand
  {
    private readonly Func<bool> canExecute;
    private readonly Func<Task> execute;

    public event EventHandler CanExecuteChanged;

    public AsyncDelegateCommand(Func<Task> execute)
      : this(execute, null)
    {
    }

    public AsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      if (canExecute == null)
      {
        return true;
      }

      return canExecute.Invoke();
    }

    public async void Execute(object parameter)
    {
      await execute.Invoke();
    }

    public void RaiseCanExecuteChanged()
    {
      if (CanExecuteChanged != null)
      {
        CanExecuteChanged(this, EventArgs.Empty);
      }
    }
  }
}
