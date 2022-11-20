using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using logcat_monitor.configuration;
using logcat_monitor.extenstions;
using logcat_monitor.services.internals;

namespace logcat_monitor.services
{
    public class WebSocketService: BackgroundService
    {
        private readonly ILogger<WebSocketService> _logger;
        private readonly WebSocketServer _wsServer;

        public WebSocketService(ILogger<WebSocketService> logger, IEventDispatch eventDispatch, IOptions<WebSocketServerConfig> config)
        {
            var wsConfig = config.Value;

            var wsPort = wsConfig?.Port ?? 9091;
            var wsIp = IPAddress.Parse(wsConfig?.IP ?? IPAddress.Any.ToString());

            _logger = logger;
            _wsServer = new WebSocketServer(wsIp,  wsPort);

            eventDispatch.onLogcatMessage += EventDispatch_onLogcatMessage;
        }

        private void EventDispatch_onLogcatMessage(object sender, DispatchEventArgs e)
        {
            _wsServer.MulticastText($"data: {e.ToJson()}");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting ...");
            var started = _wsServer.Start();

            if (started)
            {
                _logger.LogInformation("WS server started");
            }
            else
            {
                _logger.LogError("Ws server failed to start!");
            }

            return started ? Task.CompletedTask : Task.FromResult(started);
        }
    }
}

