using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Orbiter
{
    class HotKey : IDisposable
    {
        public enum Modifiers : uint
        {
            NONE = 0x0000,
            ALT = 0x0001,
            CTRL = 0x0002,
            NOREPEAT = 0x4000,
            SHIFT = 0x0004,
            WIN = 0x0008
        }

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
            RegisterHotKey(hWnd, id, modifiers, vk);
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
}
