using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RemoveShortcut
{
    class RemoveShortcut
    {
        //takes in the path where the shortcut is located as argument.
        static void Main(string[] args)
        {
            if (args.Count() == 0) return;

            string dir = string.Join(" ", args);
            const string shortcutName = "MouseJester.lnk";
            File.Delete(Path.Combine(dir, shortcutName));
        }
    }
}
