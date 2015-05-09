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
}
