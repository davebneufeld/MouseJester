using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MouseJester
{
    internal class ShowCommand : ICommand
    {
        internal void Execute(object parameter)
        {
            MainWindow.Instance.ExecuteShowEvent();
        }

        internal bool CanExecute(object parameter)
        {
            return true;
        }

        internal event EventHandler CanExecuteChanged;
    }

    internal class CloseCommand : ICommand
    {
        internal void Execute(object parameter)
        {
            MainWindow.Instance.ExecuteCloseEvent();
        }

        internal bool CanExecute(object parameter)
        {
            return true;
        }

        internal event EventHandler CanExecuteChanged;
    }
}
