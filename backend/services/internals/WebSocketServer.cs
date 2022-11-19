using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetCoreServer;

namespace logcat_monitor.services.internals
{
    public class WebSocketServer: WsServer
    {
        public WebSocketServer(IPAddress address, int port) : base(address, port)
        {
        }
        protected override TcpSession CreateSession() { return new WebSocketSession(this); }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket server caught an error with code {error}");
        }

        public void sentToClient(Guid id, String message)
        {
            FindSession(id).Send(message);
        }
    }
}

