using System;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;

namespace logcat_monitor.services.internals
{
    internal class WebSocketSession : WsSession
    {
        public WebSocketSession(WebSocketServer server) : base(server)
        {
        }

        public override void OnWsConnected(HttpRequest request)
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} connected!");
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} disconnected!");
        }


        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine($"Incoming from {Id} message: {message} ");
        }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket session caught an error with code {error}");
        }
    }
}