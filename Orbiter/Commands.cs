using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Gestr
{
    public class ShowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.ExecuteShowEvent();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class CloseCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.ExecuteCloseEvent();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
