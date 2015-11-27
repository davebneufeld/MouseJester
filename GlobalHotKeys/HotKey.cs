using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;

namespace Qaovxtazypdl.GlobalHotKeys
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
        internal const int WM_HOTKEY = 0x0312;
        internal const int TEST_ID = -2147483648;

        // Registers a hot key with Windows.
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
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

        private int _id;
        public int id
        {
            get
            {
                return _id;
            }
            internal set
            {
                _id = value;
            }
        }

        private bool disposed = false;

        public HotKey(int id, uint modifiers, uint vk)
        {
            this.id = id;
            this.disposed = false;
            this._Disabled = false;
            if (!RegisterHotKey(HotKeyWindow.Instance.hWnd, id, modifiers, vk))
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

            if (!UnregisterHotKey(HotKeyWindow.Instance.hWnd, id))
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

        public static bool isHotKeyAvilable(uint modifiers, uint vk)
        {
            if (!RegisterHotKey(HotKeyWindow.Instance.hWnd, TEST_ID, modifiers, vk))
            {
                return false;
            }
            else
            {
                UnregisterHotKey(HotKeyWindow.Instance.hWnd, TEST_ID);
                return true;
            }
        }

        public static string GetKeyComboString(uint modifiers, uint key)
        {
            string combo = "";
            if ((modifiers & (uint)ModifierKeys.Control) != 0)
            {
                combo += "CTRL+";
            }
            if ((modifiers & (uint)ModifierKeys.Alt) != 0)
            {
                combo += "ALT+";
            }
            if ((modifiers & (uint)ModifierKeys.Shift) != 0)
            {
                combo += "SHIFT+";
            }
            if (key == 0)
            {
                combo += "Not Set";
            }
            else
            {
                combo += Constants.vkeyMap[key];
            }
            return combo;
        }
    }
}
