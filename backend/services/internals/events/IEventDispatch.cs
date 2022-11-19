using System;
namespace logcat_monitor.services.internals
{
    public interface IEventDispatch
    {
        event EventHandler<DispatchEventArgs> onLogcatMessage;
        void sendLogcatMessage(DispatchEventArgs e);
    }
}

