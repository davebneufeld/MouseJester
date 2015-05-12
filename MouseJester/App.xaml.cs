using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace MouseJester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex singleInstanceLock;
        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            singleInstanceLock = new Mutex(false, "Global\\" + Constants.APP_GUID);
            if (!singleInstanceLock.WaitOne(0, false))
            {
                MessageBox.Show("Instance already running.");
                Application.Current.Shutdown();
            }
            else
            {
                MouseJester.MainWindow.Instance.Show();
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            GestureManager.Instance.Save();
            singleInstanceLock.Close();
        }
    }
}
