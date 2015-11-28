using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MouseJester
{
    public class GestureAction
    {
        public GestureAction()
        {
            this.Path = "";
            this.Arguments = "";
            this.StartIn = "";
        }

        public GestureAction(String Path, String Arguments, String StartIn) {
            this.Path = Path;
            this.Arguments = Arguments;
            this.StartIn = StartIn;
        }

        public GestureAction(GestureAction other)
        {
            this.Path = other.Path;
            this.Arguments = other.Arguments;
            this.StartIn = other.StartIn;
        }

        public String Path { get; set; }

        public String Arguments { get; set; }

        public String StartIn { get; set; }

        public void Execute() {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            if (!String.IsNullOrEmpty(this.StartIn) || String.IsNullOrEmpty(this.Path))
            {
                processStartInfo.WorkingDirectory = this.StartIn;
            }
            else
            {
                processStartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(this.Path);
            }
            processStartInfo.FileName = this.Path;
            processStartInfo.Arguments = this.Arguments;
            processStartInfo.CreateNoWindow = true;

            try
            {
                Process myProcess = Process.Start(processStartInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
