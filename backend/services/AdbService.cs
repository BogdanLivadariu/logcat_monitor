using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using logcat_monitor.helpers;
using logcat_monitor.services.internals;
using System.Collections.Generic;
using logcat_monitor.configuration;
using Microsoft.Extensions.Options;

namespace logcat_monitor.services
{
    public class AdbService : BackgroundService
    {
        private readonly Dictionary<string, Thread> logcatMonitoring = new Dictionary<string, Thread>();

        private readonly string ADB_BIN = "/usr/bin/adb";

        private readonly ILogger<AdbService> _logger;

        private readonly ProcessHelper _procHelper;

        private readonly AdbConfig _config;

        public AdbService(ILogger<AdbService> logger, IOptions<AdbConfig> config)
        {
            _logger = logger;
            _procHelper = new ProcessHelper(_logger);

            _config = config.Value;

            if (!string.IsNullOrEmpty(_config.BinPath))
            {
                ADB_BIN = _config.BinPath;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting");

            connectToAdbDevices(stoppingToken);
            monitorAdbDevices(stoppingToken);

            return Task.CompletedTask;
        }

        private void connectToAdbDevices(CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                if (_config.Devices?.Count > 0)
                {
                    foreach (var device in _config.Devices)
                    {
                        var output = _procHelper.ConnectToAdbDevice(ADB_BIN, device, stoppingToken);
                        _logger.LogInformation("Connection state from {device}: {output}", device, output);
                    }
                }
                else
                {
                    _logger.LogInformation("No devices provided ...");
                }
            }, stoppingToken);
        }

        private void monitorAdbDevices(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var devices = _procHelper.GetAdbDevices(ADB_BIN, stoppingToken);

                    devices.ForEach(device =>
                    {
                        if (!logcatMonitoring.ContainsKey(device))
                        {
                            var serial = _procHelper.GetAdbDeviceSerialNumber(ADB_BIN, device, stoppingToken);

                            var logcatDir = $"./logcat/{device.Split(":")[0]}-{serial}";


                            if (!Directory.Exists(logcatDir))
                            {
                                Directory.CreateDirectory(logcatDir);
                            }

                            monitorAdbDeviceLogcat(logcatDir, device, stoppingToken);
                        }
                        else
                        {
                            _logger.LogInformation("{device} is already monitored ...", device);
                        }
                    });

                    await Task.Delay(5000);
                };
            });
        }

        private void monitorAdbDeviceLogcat(string logcatDir, string device, CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                logcatMonitoring.Add(device, Thread.CurrentThread);

                void logcat(string line)
                {
                    File.AppendAllText($"{logcatDir}/{DateTime.Now.ToString("yyyy-MM-dd")}.log", line + Environment.NewLine);
                    GC.Collect();
                }

                _procHelper.ClearAdbLogcat(ADB_BIN, device, stoppingToken);
                _procHelper.MonitorAdbDeviceLogcat(ADB_BIN, device, logcat, stoppingToken);

                _logger.LogWarning("Logcat session finished for ... {device}", device);

                logcatMonitoring.Remove(device);
            }, stoppingToken);
        }
    }
}

