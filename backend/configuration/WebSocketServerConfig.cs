using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logcat_monitor.configuration
{
    public class WebSocketServerConfig
    {
        public const string SECTION_NAME = "WebSocketServer";

        public bool Enabled { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }
    }
}
