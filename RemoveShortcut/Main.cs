using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace RemoveShortcut
{
    class RemoveShortcut
    {
        [DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        //takes in the path where the shortcut is located as argument.
        static void Main(string[] args)
        {
            if (args.Count() == 0) return;

            string dir = string.Join(" ", args);
            const string shortcutName = "MouseJester.lnk";
            File.Delete(Path.Combine(dir, shortcutName));

            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
