using System;
namespace logcat_monitor.services.internals.events
{
    public class EventDispatch : IEventDispatch
    {
        public event EventHandler<DispatchEventArgs> onLogcatMessage;

        public void sendLogcatMessage(DispatchEventArgs e)
        {
            Console.WriteLine("sending message ...");
            onLogcatMessage?.Invoke(this, e);
        }
    }
}

