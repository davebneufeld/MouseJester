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

namespace Orbiter
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

        const int GESTURE_INPUT_ID = 1;
        List<HotKey> DefinedHotKeys;

        public MainWindow()
        {
            InitializeComponent();
            new GestureDrawer();
            DefinedHotKeys = new List<HotKey>();
            ShowEvent += ShowEventHandler;
            CloseEvent += CloseEventHandler;

            DefinedHotKeys.Add(new HotKey(GESTURE_INPUT_ID, (uint)KeyModifier.Ctrl, (uint)VirtualKey.B));
            DefinedHotKeys.Add(new HotKey(3, (uint)KeyModifier.Ctrl, (uint)VirtualKey.C));

            foreach(HotKey hkey in DefinedHotKeys) 
            {
                hkey.AddHotKeyHandler(HotkeyHandler);
            }
            
        }

        protected override void OnClosed(EventArgs e)
        {
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

        private void HotkeyHandler(Object sender, HotKeyEventArgs e)
        {
            int id = e.id;
            MessageBox.Show(HotKeyWindow.Instance, "Hello World. " + id);
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
