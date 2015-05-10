
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Messaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orbiter
{
    public class HotKey : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event HotkeyHandlerDelegate HotKeyPressedEvent;

        public int id;
        private bool disposed = false;
        private IntPtr hWnd;

        public HotKey(int id, uint modifiers, uint vk) 
        {
            this.id = id;
            hWnd = (new WindowInteropHelper(HotKeyWindow.Instance)).Handle;
            if(!RegisterHotKey(hWnd, id, modifiers, vk))
            {
                MessageBox.Show(HotKeyWindow.Instance, "Failed to register hotkey with ID: " + id);
                return;
            }
            HotKeyWindow.RegisterHotKey(this);
        }

        public HotKey(int id, uint modifiers, uint vk, HotkeyHandlerDelegate HotKeyHandler) : this(id, modifiers, vk)
        {
            AddHotKeyHandler(HotKeyHandler);
        }

        public void Dispose() 
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            if (disposed)
                return;

            if(!UnregisterHotKey(hWnd, id))
            {
                Console.WriteLine("Could not unregister the hotkey.");
            }
            disposed = true;
        }

        public void AddHotKeyHandler(HotkeyHandlerDelegate HotKeyHandler)
        {
            HotKeyPressedEvent += HotKeyHandler;
        }

        internal void RaiseHotKeyEvent()
        {
            HotkeyHandlerDelegate handler = HotKeyPressedEvent;
            if (handler != null)
            {
                handler(this, new HotKeyEventArgs(id));
            }
        }
    }

    public class HotKeyEventArgs : EventArgs
    {
        public int id;
        public HotKeyEventArgs(int id) : base()
        {
            this.id = id;
        }
    }

    public delegate void HotkeyHandlerDelegate(Object sender, HotKeyEventArgs e);

    public partial class HotKeyWindow : Window
    {
        private static HotKeyWindow _instance = null;
        private static List<HotKey> registeredKeys = new List<HotKey>();

        public static HotKeyWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HotKeyWindow();
                }
                return _instance;
            }
        }

        private HotKeyWindow() : base()
        {
            //make this window invisible.
            InitializeComponent();
            Show();
        }

        const int WM_HOTKEY = 0x0312;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY:
                    //check hotkey register ID
                    int id = wParam.ToInt32();

                    //raise the event with the hotkey ID
                    foreach (HotKey hkey in registeredKeys)
                    {
                        if (hkey.id == id)
                        {
                            hkey.RaiseHotKeyEvent();
                        }
                    }

                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        internal static void RegisterHotKey(HotKey hkey)
        {
            registeredKeys.Add(hkey);
        }
    }
}
