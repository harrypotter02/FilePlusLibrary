using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlusLibrary
{
    public class MTraceListener : TextWriterTraceListener
    {
        public MTraceListener(TextWriter tw):base(tw)
        {
        
        }

        // called (in debug-mode) when Debug.WriteLine() is called
        public override void WriteLine(string message)
        {
            // handle/output "message" properly
            string t = DateTime.Now.ToString("MM_dd_HH:mm:ss ");
            message = t + message;
            base.WriteLine(message);
        }
    }

}
