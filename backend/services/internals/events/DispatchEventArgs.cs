using System;

namespace logcat_monitor.services.internals
{
    public class DispatchEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string SerialNumber { get; set; }
    }
}