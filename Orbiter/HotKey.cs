using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Gestr
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
            if (!RegisterHotKey(hWnd, id, modifiers, vk))
            {
                MessageBox.Show(HotKeyWindow.Instance, "Failed to register hotkey with ID: " + id);
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
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (!UnregisterHotKey(hWnd, id))
            {
                Console.WriteLine("Could not unregister the hotkey.");
            }
            disposed = true;
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
}
