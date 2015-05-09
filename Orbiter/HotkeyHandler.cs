using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Messaging;

namespace Orbiter
{                
        public class HotKeyEventArgs : EventArgs 
        {
            public int id;
            public HotKeyEventArgs(int id) : base()
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

            private HotKeyWindow() : base()
            {
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

                        //raise the event
                        HotkeyHandlerDelegate handler = HotKeyPressedEvent;
                        if(handler != null)
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
