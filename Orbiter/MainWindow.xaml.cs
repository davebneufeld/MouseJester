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

namespace Gestr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler ShowEvent;
        public static event EventHandler CloseEvent;

        public static void ExecuteShowEvent()
        {
            EventHandler handler = MainWindow.ShowEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        public static void ExecuteCloseEvent()
        {
            EventHandler handler = MainWindow.CloseEvent;
            if (handler != null)
            {
                handler(null, new EventArgs());
            }
        }

        List<HotKey> DefinedHotKeys;

        public MainWindow()
        {
            InitializeComponent();
            DefinedHotKeys = new List<HotKey>();
            ShowEvent += ShowEventHandler;
            CloseEvent += CloseEventHandler;

            GestureManager.Instance.hkey = new HotKey(Constants.GESTURE_INPUT_ID, (uint)(KeyModifier.Ctrl), (uint)VirtualKey.B, GestureManager.Instance.HotKeyHandler);
            GestureManager.Instance.Load();
            DefinedHotKeys.Add(GestureManager.Instance.hkey);        
        }

        protected override void OnClosed(EventArgs e)
        {
            GestureManager.Instance.Save();
            base.OnClosed(e);
            foreach (HotKey hkey in DefinedHotKeys)
            {
                hkey.Dispose();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
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
