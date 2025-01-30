using System.Net.NetworkInformation;
using Microsoft.Win32;
using DotNetEnv;
namespace BatteryMonitor
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Env.Load();
            ApplicationConfiguration.Initialize();
            Application.Run(new BatteryMonitorApp());
        }
    }
    internal class BatteryMonitorApp : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly System.Windows.Forms.Timer _checkTimer;
        private const int MinBattery = 45;
        private const int MaxBattery = 80;
        private readonly string[] _allowedNetworks;
        private bool _isMonitoringActive;
        private string _lastNetwork = "";
        public BatteryMonitorApp()
        {
            SetStartup(true);
            var trayMenu = new ContextMenuStrip();
            var toggleMonitoringItem = new ToolStripMenuItem("Activate", null, ToggleMonitoring!);
            trayMenu.Items.Add(toggleMonitoringItem);
            trayMenu.Items.Add("Quit", null, OnExit!);
            _trayIcon = new NotifyIcon()
            {
                Icon = new Icon("Resources/icon.ico"),
                ContextMenuStrip = trayMenu,
                Text = "Battery Monitoring",
                Visible = true
            };
            _checkTimer = new System.Windows.Forms.Timer();
            _checkTimer.Interval = 15000;
            _checkTimer.Tick += (_, _) => CheckConditions();
            _checkTimer.Start();
            var allowedNetworksString = Env.GetString("ALLOWED_NETWORKS");
            if (string.IsNullOrEmpty(allowedNetworksString))
            {
                _allowedNetworks = new string[] { }; 
                Console.WriteLine("Warning: ALLOWED_NETWORKS is not set in .env");
            }
            else
            {
                _allowedNetworks = allowedNetworksString
                    .Split(',')
                    .Select(n => n.Trim())
                    .ToArray();
            }
        }
        private void ToggleMonitoring(object sender, EventArgs e)
        {
            _isMonitoringActive = !_isMonitoringActive;
            ((ToolStripMenuItem)sender).Text = _isMonitoringActive ? "Deactivate" : "Activate";
        }
        private bool CheckNetwork(string currentNetwork)
        {
            if (currentNetwork == _lastNetwork) return true;
            _lastNetwork = currentNetwork;
            return Array.Exists(_allowedNetworks, net => net == currentNetwork);
        }
        private void CheckConditions()
        {
            if (!_isMonitoringActive) return;
            var currentNetwork = GetCurrentNetworkId();
            var isCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
            if (!isCharging)
            {
                if (!CheckNetwork(currentNetwork)) return;
            }

            ExecuteMainSystem(isCharging);
        }
        private static void ExecuteMainSystem(bool isCharging)
        {
            var batteryLevel = SystemInformation.PowerStatus.BatteryLifePercent * 100;
            switch (batteryLevel)
            {
                case < MinBattery:
                    Console.WriteLine("System start");
                    // Call API
                    break;
                case > MaxBattery:
                    if (isCharging)
                    {
                        Console.WriteLine("System stop");
                        // Call API
                    }
                    break;
                default:
                    Console.WriteLine($"Everything good, Battery level : {batteryLevel}");
                    break;
            }
        }
        private static string GetCurrentNetworkId()
        {
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.OperationalStatus == OperationalStatus.Up &&
                    (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    return netInterface.Name;
                }
            }

            return "No connexion";
        }
        private static void SetStartup(bool enable)
        {
            const string appName = "BatteryMonitorApp";
            var exePath = Application.ExecutablePath;
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (enable)
            {
                key!.SetValue(appName, exePath);
            }
            else
            {
                key!.DeleteValue(appName, false);
            }
        }
        private void OnExit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}