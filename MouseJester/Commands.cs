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
        public void Execute(object parameter)
        {
            MainWindow.Instance.ExecuteShowEvent();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    internal class CloseCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.Instance.ExecuteCloseEvent();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
