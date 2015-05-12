using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MouseJester
{
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

    public class HotKey : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event HotkeyHandlerDelegate HotKeyPressedEvent;

        private bool _Disabled;
        public bool Disabled
        {
            get
            {
                return _Disabled;
            }
            set
            {
                _Disabled = value;
            }
        }

        public int id;
        private bool disposed = false;
        private IntPtr hWnd;

        public HotKey(int id, uint modifiers, uint vk)
        {
            this.id = id;
            this.disposed = false;
            this._Disabled = false;
            hWnd = (new WindowInteropHelper(HotKeyWindow.Instance)).Handle;
            if (!RegisterHotKey(hWnd, id, modifiers, vk))
            {
                MessageBox.Show("Failed to register hotkey with ID: " + id + " with error = " + Marshal.GetLastWin32Error());
                return;
            }
            HotKeyWindow.RegisterHotKey(this);
        }

        public HotKey(int id, uint modifiers, uint vk, HotkeyHandlerDelegate HotKeyHandler)
            : this(id, modifiers, vk)
        {
            HotKeyPressedEvent += HotKeyHandler;
        }

        public void Dispose()
        {
            Dispose(true);
            _Disabled = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (!UnregisterHotKey(hWnd, id))
            {
                MessageBox.Show("Could not unregister the hotkey. Error = " + Marshal.GetLastWin32Error());
            }
            disposed = true;
        }

        internal void RaiseHotKeyEvent()
        {
            HotkeyHandlerDelegate handler = HotKeyPressedEvent;
            if (handler != null && !Disabled)
            {
                handler(this, new HotKeyEventArgs(id));
            }
        }
    }
}
