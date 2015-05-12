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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace MouseJester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _Instance = null;
        public static MainWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MainWindow();
                }
                return _Instance;
            }
        }

        public event EventHandler ShowEvent;
        public event EventHandler CloseEvent;

        private MainWindow() : base()
        {
            InitializeComponent();
            ShowEvent += ShowEventHandler;
            CloseEvent += CloseEventHandler;

            GestureManager.Instance.Load();
            GestureManager.Instance.hkey = new HotKey(Constants.GESTURE_INPUT_ID, (uint)(ModifierKeys.Control), (uint)VirtualKey.B, GestureManager.Instance.HotKeyHandler);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        public void ExecuteShowEvent()
        {
            EventHandler handler = ShowEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        public void ExecuteCloseEvent()
        {
            EventHandler handler = CloseEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        private void ShowEventHandler(object sender, EventArgs e)
        {
            this.Show();
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseEventHandler(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
