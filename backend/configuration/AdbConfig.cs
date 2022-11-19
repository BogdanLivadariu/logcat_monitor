using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logcat_monitor.configuration
{
    public class AdbConfig
    {
        public const string SECTION_NAME = "Adb";

        public string BinPath { get; set; }
        public IList<string> Devices { get; set; }
        public bool Enabled { get; set; }
    }
}
