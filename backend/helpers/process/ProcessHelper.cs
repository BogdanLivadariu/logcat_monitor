using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace logcat_monitor.helpers
{
    public class ProcessHelper
    {
        private readonly ILogger _logger;

        public ProcessHelper(ILogger logger)
        {
            _logger = logger;
        }

        private void RunCommand(string cmd, string args, CancellationToken stoppingToken, Action<string> cb = null, bool waitForExit = false)
        {
            var process = new Process();

            process.StartInfo.FileName = cmd;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.OutputDataReceived += (sender, data) => cb?.Invoke(data.Data);

            var started = process.Start();

            void killSwitch()
            {
                _logger.LogWarning("KillSwitch on: {cmd} {args}", cmd, args);
                process.Kill(true);
            }

            using (stoppingToken.Register(killSwitch))
            {
                _logger.LogInformation("Started({started}), wait to exit({waitForExit}): command: '{cmd} {args}'",
                    started, waitForExit, cmd, args);

                process.BeginOutputReadLine();

                if (waitForExit)
                {
                    process.WaitForExit();
                }
            }
        }

        public string GetCommandOutput(string cmd, string args, CancellationToken stoppingToken)
        {
            var sb = new StringBuilder();
            Action<string> action = delegate (string line)
            {
                sb.Append(line);
            };

            RunCommand(cmd, args, stoppingToken, action, true);

            return sb.ToString();

        }

        public string ConnectToAdbDevice(string adb, string deviceIp, CancellationToken stoppingToken)
        {
            _logger.LogInformation("ConnectToAdbDevice: {deviceIp}", deviceIp);

            var result = new List<string>();

            void action(string line)
            {
                result.Add(line);

            }

            RunCommand(adb, $"connect {deviceIp}", stoppingToken, action, true);

            return string.Join("", result).Trim();
        }

        public List<string> GetAdbDevices(string adb, CancellationToken stoppingToken)
        {
            _logger.LogInformation("GetAdbDevices");

            var result = new List<string>();

            Action<string> action = delegate (string line)
            {
                if (!string.IsNullOrEmpty(line) && line.EndsWith("device", StringComparison.CurrentCultureIgnoreCase))
                {
                    var deviceAddr = line.Split("\t")?[0];
                    result.Add(deviceAddr);
                }
            };

            RunCommand(adb, "devices", stoppingToken, action, true);

            return result;
        }

        public void MonitorAdbDeviceLogcat(string adb, string deviceAddr, Action<string> cb, CancellationToken stoppingToken)
        {
            _logger.LogInformation("GetAdbDeviceLogcat");

            void action(string line)
            {
                cb?.Invoke(line);
            }

            RunCommand(adb, $"-s {deviceAddr} logcat", stoppingToken, action, true);
        }

        public void ClearAdbLogcat(string adb, string deviceAddr, CancellationToken stoppingToken)
        {
            _logger.LogInformation("ClearAdbLogcat");

            GetCommandOutput(adb, $"-s {deviceAddr} logcat -c", stoppingToken);
        }

        public string GetAdbDeviceSerialNumber(string adb, string deviceAddr, CancellationToken stoppingToken)
        {
            _logger.LogInformation("GetAdbDeviceSerialNumber");

            return GetCommandOutput(adb, $"-s {deviceAddr} shell getprop ro.customer.serialno", stoppingToken);
        }
    }
}

