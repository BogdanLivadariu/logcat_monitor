using System;
using Newtonsoft.Json;
using logcat_monitor.services.internals;

namespace logcat_monitor.extenstions
{
    public static class DispatchEventArgsExtensions
    {
        public static String ToJson(this DispatchEventArgs data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}

