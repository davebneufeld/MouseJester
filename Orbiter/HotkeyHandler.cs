using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Messaging;

namespace Orbiter
{
    class HotKey : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        int id;
        bool disposed = false;
        IntPtr hWnd;

        public HotKey(int id, uint modifiers, uint vk) 
        {
            this.id = id;
            hWnd = (new WindowInteropHelper(HotKeyWindow.Instance)).Handle;
            if(!RegisterHotKey(hWnd, id, modifiers, vk))
            {
                Console.WriteLine("Failed to register hotkey with ID: " + id);
            }
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
    }

    public class HotKeyEventArgs : EventArgs
    {
        public int id;
        public HotKeyEventArgs(int id)
            : base()
        {
            this.id = id;
        }
    }

    public delegate void HotkeyHandlerDelegate(Object sender, HotKeyEventArgs e);

    public class HotKeyWindow : Window
    {
        private static HotKeyWindow _instance = null;
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

        private HotKeyWindow()
            : base()
        {
            //make this window invisible.
            Width = 0;
            Height = 0;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            ShowActivated = false;
            Show();
        }

        const int WM_HOTKEY = 0x0312;
        public event HotkeyHandlerDelegate HotKeyPressedEvent;

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
                    HotkeyHandlerDelegate handler = HotKeyPressedEvent;
                    if (handler != null)
                    {
                        handler(this, new HotKeyEventArgs(id));
                    }

                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        public void AddHotKeyHandler(HotkeyHandlerDelegate HotKeyHandler)
        {
            HotKeyPressedEvent += HotKeyHandler;
        }
    }
}
