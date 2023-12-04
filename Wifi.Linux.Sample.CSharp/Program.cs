using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace Wifi.Linux.Sample.CSharp
{
    public class Wifi
    {
        public required string BSSID { get; set; }
        public required string SSID { get; set; }
    }

    [DBusInterface("fi.w1.wpa_supplicant1")]
    interface IWpaSupplicant1 : IDBusObject
    {
        Task<ObjectPath> CreateInterfaceAsync(IDictionary<string, object> args);
        Task<ObjectPath> GetInterfaceAsync(string ifname);
        Task<T> GetAsync<T>(string prop);
    }

    [DBusInterface("fi.w1.wpa_supplicant1.Interface")]
    interface IWpaSupplicant1Interface : IDBusObject
    {
        Task ScanAsync(IDictionary<string, string> args);
    }

    [Dictionary]
    public class WpaSupplicant1Properties
    {
        private ObjectPath[] _Interfaces = default(ObjectPath[]);
        public ObjectPath[] Interfaces
        {
            get
            {
                return _Interfaces;
            }

            set
            {
                _Interfaces = (value);
            }
        }

        private string[] _Capabilities = default(string[]);
        public string[] Capabilities
        {
            get
            {
                return _Capabilities;
            }

            set
            {
                _Capabilities = (value);
            }
        }
    }

    [Dictionary]
    public class WpaSupplicant1InterfaceProperties
    {
        private IDictionary<string, object> _Capabilities = default(IDictionary<string, object>);
        public IDictionary<string, object> Capabilities
        {
            get
            {
                return _Capabilities;
            }

            set
            {
                _Capabilities = (value);
            }
        }

        private string _Ifname = default(string);
        public string Ifname
        {
            get
            {
                return _Ifname;
            }

            set
            {
                _Ifname = (value);
            }
        }
    }

    internal interface IObjectManagerProperties
    {
        IDictionary<string, IDictionary<string, object>> GetProperties();
    }

    public static class PropertyAccessExtensions
    {
        public static T ReadProperty<T>(this object o, string prop)
        {
            var propertyValue = o.GetType().GetProperty(prop)?.GetValue(o);
            return (T)propertyValue;
        }

        public static object ReadProperty(this object o, string prop)
        {
            return o.GetType().GetProperty(prop)?.GetValue(o);
        }


        public static Task SetProperty(this object o, string prop, object val)
        {
            o.GetType().GetProperty(prop)?.SetValue(o, val);
            return Task.CompletedTask;
        }
    }

    public abstract class PropertiesBase<TV>
    {
        protected readonly TV Properties;

        protected PropertiesBase(ObjectPath objectPath, TV properties)
        {
            ObjectPath = objectPath;
            Properties = properties;
        }

        public ObjectPath ObjectPath { get; }

        public Task<object> GetAsync(string prop)
        {
            return Task.FromResult(Properties.ReadProperty(prop));
        }
        public Task<T> GetAsync<T>(string prop)
        {
            return Task.FromResult(Properties.ReadProperty<T>(prop));
        }

        public Task<TV> GetAllAsync()
        {
            return Task.FromResult(Properties);
        }

        public Task SetAsync(string prop, object val)
        {
            return Properties.SetProperty(prop, val);
        }

        public Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler)
        {
            return SignalWatcher.AddAsync(this, nameof(OnPropertiesChanged), handler);
        }

        public event Action<PropertyChanges> OnPropertiesChanged;
    }

    internal class WpaSupplicant1 : PropertiesBase<WpaSupplicant1Properties>, IWpaSupplicant1, IObjectManagerProperties
    {
        public WpaSupplicant1(ObjectPath objectPath, WpaSupplicant1Properties wpaSupplicant1Properties)
            : base(objectPath, wpaSupplicant1Properties)
        {

        }

        public IDictionary<string, IDictionary<string, object>> GetProperties()
        {
            return new Dictionary<string, IDictionary<string, object>>()
            {
                {
                    "org.bluez.GattDescriptor1", new Dictionary<string, object>
                    {
                        { "Interfaces", Properties.Interfaces },
                        { "Capabilities", Properties.Capabilities },
                    }
                }
            };
        }

        public Task<ObjectPath> CreateInterfaceAsync(IDictionary<string, object> args)
        {
            var inter = new WpaSupplicantInterface(ObjectPath, new WpaSupplicant1InterfaceProperties() { Ifname = "wlan0" });
            return Task.FromResult(inter.ObjectPath);
        }

        public Task<ObjectPath> GetInterfaceAsync(string ifname)
        {
            throw new NotImplementedException();
        }
    }

    internal class WpaSupplicantInterface : PropertiesBase<WpaSupplicant1InterfaceProperties>, IWpaSupplicant1Interface, IObjectManagerProperties
    {
        public WpaSupplicantInterface(ObjectPath objectPath, WpaSupplicant1InterfaceProperties wpaSupplicant1InterfaceProperties)
            : base(objectPath, wpaSupplicant1InterfaceProperties)
        {

        }

        public IDictionary<string, IDictionary<string, object>> GetProperties()
        {
            return new Dictionary<string, IDictionary<string, object>>()
            {
                {
                    "fi.w1.wpa_supplicant1.Interface", new Dictionary<string, object>
                    {
                        { "Capabilities", Properties.Capabilities },
                    }
                }
            };
        }

        public Task ScanAsync(IDictionary<string, string> args)
        {
            throw new NotImplementedException();
        }
    }

    internal class Program
    {
        #region Interop  

        [DllImport("libExecuteCommand.so", EntryPoint = "exec")]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string exec([MarshalAs(UnmanagedType.LPStr)] string command);


        [DllImport("libWifi.Linux.Core.so", EntryPoint = "ExecuteCommand")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ExecuteCommand(string command, ref OUTPUT output);

        [DllImport("libWifi.Linux.Core.so", EntryPoint = "SystemCommand")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int SystemCommand(string command, ref OUTPUT output);


        [DllImport("libWifi.Linux.Core.so", EntryPoint = "ScanWifis")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ScanWifis(ref SCAN scan);

        [StructLayout(LayoutKind.Sequential)]
        public struct SCAN
        {
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 10)]
            public wireless_scan[] scans;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 10)]
            public WIFI[] wifi;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct OUTPUT
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string output_string;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WIFI
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
            public string essid;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct wireless_config
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public char[] name;                           // Wireless/protocol name
            public int has_nwid;
            //public iwparam nwid;                          // Network ID
            public int has_freq;
            public double freq;                           // Frequency/channel
            public int freq_flags;
            public int has_key;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] key;                            // Encoding key used
            public int key_size;                          // Number of bytes
            public int key_flags;                         // Various flags
            public int has_essid;
            public int essid_on;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
            public string essid;                         // ESSID (extended network)
            public int has_mode;
            public int mode;                              // Operation mode
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct wireless_scan
        {
            public IntPtr next; // Linked list
            public int has_ap_addr;
            public sockaddr ap_addr; // Access point address
            public wireless_config b; // Basic information
            //public iwstats stats; // Signal strength
            public int has_stats;
            //public iwparam maxbitrate; // Max bit rate in bps
            public int has_maxbitrate;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct sockaddr
        {
            public ushort sa_family; // Common data: address family
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] sa_data;   // Address data
        };
        #endregion

        string sblock = "";

        private async Task<string> Exec(string exe, string args)
        {
            Console.WriteLine(exe + " " + args);

            var sb = new StringBuilder();
            var startinfo = new ProcessStartInfo(exe, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var ret = new Process
            {
                StartInfo = startinfo,
                EnableRaisingEvents = true
            };

            ret.OutputDataReceived += (sender, eventArgs) =>
            {
                lock (sblock)
                {
                    var data = eventArgs.Data;
                    if (string.IsNullOrEmpty(data))
                    {
                        return;
                    }
                    sb.AppendLine(data);
                    Debug.Write(data);
                }
            };

            ret.ErrorDataReceived += (sender, eventArgs) =>
            {
                lock (sblock)
                {
                    var data = eventArgs.Data;
                    if (string.IsNullOrWhiteSpace(data))
                    {
                        return;
                    }
                    sb.AppendLine("ERROR: " + data);
                    Debug.Write(data);
                }
            };
            Debug.Assert(ret != null, "ret != null");

            ret.Start();
            ret.BeginOutputReadLine();
            ret.BeginErrorReadLine();
            await ret.WaitForExitAsync();

            lock (sblock)
            {
                return (sb.ToString());
            }
        }

        public async Task<string> ExecuteCommandAsync(string command)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            await proc.WaitForExitAsync();

            while (!proc.StandardOutput.EndOfStream)
            {
                stringBuilder.AppendLine(proc.StandardOutput.ReadLine());
                Console.WriteLine(proc.StandardOutput.ReadLine());
            }

            return stringBuilder.ToString();
        }

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Program p = new Program();

                var oo1 = await p.Exec("wpa_cli", "scan -i wlan0");
                Console.WriteLine(oo1);
                var oo = await p.Exec("wpa_cli", "scan_results -i wlan0");
                //Console.WriteLine(oo);

                var lines = oo.Split('\n');

                string[] titles = new string[4];
                string[][] wifiLines = new string[30][];

                int i = 0;
                foreach (var line in lines)
                {
                    if (i == 0)
                        titles = line.Split(" / ");
                    else
                    {
                        wifiLines[i - 1] = (line.Split("\t"));
                    }

                    i++;
                }

                Console.WriteLine("Titles: " + string.Join(" ", titles));

                List<Wifi> wifis = new List<Wifi>();

                foreach (var wifi in wifiLines)
                {
                    if (wifi?.Length > 0)
                    {
                        wifis.Add(new Wifi { BSSID = wifi[0], SSID = wifi.Last() });
                        Console.WriteLine(wifi.Last());
                    }
                }

                var selectedWifi = wifis.FirstOrDefault(e => e.SSID.Contains("Guillaume"));

                if (selectedWifi != null)
                {
                    //var o3 = await p.Exec("wpa_passphrase", $"\"{selectedWifi.SSID}\" \"0123456789\"");
                    //Console.WriteLine(o3);

                    //File.WriteAllText("mirror_wpa_supplicant.conf", o3);

                    var o3 = await p.Exec("wpa_cli", "remove_network all");
                    Console.WriteLine("Execute called : " + o3);

                    o3 = await p.Exec("wpa_cli", "add_network");
                    Console.WriteLine("Execute called : " + o3);

                    var indice = o3.Split("\n")[1].Replace("\n", "");

                    o3 = await p.Exec($"/bin/bash", $"-c \"wpa_cli set_network {indice} ssid '\\\"{selectedWifi.SSID}\\\"'\"");
                    Console.WriteLine("Execute called : " + o3);

                    //o3 = await p.Exec("wpa_cli", $"set_network {indice} key_mgmt WPA-PSK");
                    //Console.WriteLine("Execute called : " + o3);

                    var pwd = "0123456789";
                    o3 = await p.Exec($"/bin/bash", $"-c \"wpa_cli set_network {indice} psk '\\\"{pwd}\\\"'\"");
                    Console.WriteLine("Execute called : " + o3);

                    o3 = await p.Exec("wpa_cli", $"enable_network {indice}");
                    Console.WriteLine("Execute called : " + o3);

                    o3 = await p.Exec("wpa_cli", $"select_network {indice}");
                    Console.WriteLine("Execute called : " + o3);

                    o3 = await p.Exec("wpa_cli", $"save_config");
                    Console.WriteLine("Execute called : " + o3);

                    o3 = await p.Exec("wpa_cli", $"reconfigure");
                    Console.WriteLine("Execute called : " + o3);

                    o3 = await p.Exec("reboot", $"");
                    Console.WriteLine("Execute called : " + o3);

                    //var output = await p.Exec("wpa_supplicant", $"-i wlan0 -c mirror_wpa_supplicant.conf");
                    //Console.WriteLine(output);

                    //output = await p.Exec($"dhclient", "-r");
                    //Console.WriteLine("dhclient called : " + output);

                    //output = await p.Exec("dhclient", "wlan0");
                    //Console.WriteLine("dhclient 2 called : " + output);

                }
                else
                {
                    Console.WriteLine("Not found");
                }

            }).Wait();

            //    var command = ("wpa_cli -i wlan0 scan");


            //Process proc = new Process();
            //proc.StartInfo.FileName = "/bin/bash";
            //proc.StartInfo.Arguments = "-c \" " + command + " \"";
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.RedirectStandardOutput = true;
            //proc.OutputDataReceived += Proc_OutputDataReceived;
            //proc.Start();

            //while (!proc.StandardOutput.EndOfStream)
            //{
            //    Console.WriteLine(proc.StandardOutput.ReadLine());
            //}






            //Console.WriteLine("Press a key to start");
            //Console.ReadLine();

            ////Task.Run(async () =>
            ////{
            ////    var connection = new Connection(Address.System);
            ////    await connection.ConnectAsync();

            ////    //var systemConnection = Connection.System;
            ////    Console.WriteLine("1");
            ////    var networkManager = connection.CreateProxy<IWpaSupplicant1>("fi.w1.wpa_supplicant1", "/fi/w1/wpa_supplicant1");

            ////    //var ii = await networkManager.CreateInterfaceAsync(new Dictionary<string, object> { { "Ifname", "wlan0" } });
            ////    //Console.WriteLine($"Interface : {ii.ToString()}");
            ////    //var ii2 = ii;
            ////    var prop = await networkManager.GetAsync<ObjectPath[]>("Interfaces");
            ////    Console.WriteLine(prop.Length);
            ////    Console.WriteLine("2");



            ////    var tt = await networkManager.GetInterfaceAsync("wlan0");
            ////    var t = tt;


            ////}).Wait();

            //OUTPUT output = new OUTPUT();
            //ExecuteCommand("wpa_cli -i wlan0 scan", ref output);
            //Console.WriteLine("Execute called : " + output.output_string);

            //ExecuteCommand("wpa_cli scan_results -i wlan0", ref output);
            //Console.WriteLine("Execute called : " + output.output_string);


            //var lines = output.output_string.Split('\n');

            //string[] titles = new string[4];
            //string[][] wifiLines = new string[20][];

            //int i = 0;
            //foreach (var line in lines)
            //{
            //    if (i == 0)
            //        titles = line.Split(" / ");
            //    else
            //    {
            //        wifiLines[i - 1] = (line.Split("\t"));
            //    }

            //    i++;
            //}

            //Console.WriteLine("Titles: " + string.Join(" ", titles));

            //List<Wifi> wifis = new List<Wifi>();

            //foreach (var wifi in wifiLines)
            //{
            //    if (wifi?.Length > 0)
            //    {
            //        wifis.Add(new Wifi { BSSID = wifi[0], SSID = wifi.Last() });
            //        Console.WriteLine(wifi.Last());
            //    }
            //}

            //var selectedWifi = wifis.FirstOrDefault(e => e.SSID.Contains("Guillaume"));

            //if (selectedWifi != null)
            //{
            //    //ExecuteCommand("sudo chmod +x /etc/wpa_supplicant.conf", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);
            //    //SystemCommand("ee", ref output);
            //    //Console.WriteLine("System called");
            //    //Console.WriteLine("System called : " + output.output_string);

            //    //ExecuteCommand("sudo ip link set wlan0 down", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);

            //    //ExecuteCommand("sudo ip link set wlan0 up", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);

            //    //WORKING
            //    ExecuteCommand($"wpa_supplicant -i wlan0 -c < wpa_passphrase \"{selectedWifi.SSID}\" \"0123456789\"", ref output);
            //    Console.WriteLine("Execute called : " + output.output_string);

            //    //ExecuteCommand($"wpa_passphrase \"{selectedWifi.SSID}\" \"0123456789\" > temp.conf", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);

            //    //ExecuteCommand($"wpa_supplicant -i wlan0 -c /etc/temp.conf", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);

            //    ExecuteCommand($"dhclient -r", ref output);
            //    Console.WriteLine("Execute called : " + output.output_string);

            //    SystemCommand("dhclient wlan0", ref output);
            //    Console.WriteLine("Execute called : " + output.output_string);

            //    //ExecuteCommand($"dhclient wlan0", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);


            //    //ExecuteCommand("wpa_cli add_network -i wlan0", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);

            //    //ExecuteCommand($"wpa_cli set_network {output.output_string.Replace("\n", "")} ssid \"{selectedWifi.SSID}\" -i wlan0", ref output);
            //    //Console.WriteLine("Execute called : " + output.output_string);
            //}
            //else
            //{
            //    Console.WriteLine("Not found");
            //}

            //SCAN scan = new SCAN();
            //var w = ScanWifis(ref scan);
            //Console.WriteLine("Wrapper called : " + w);

            //if (w == 1)
            //{
            //    foreach (var s in scan.scans)
            //    {
            //        Console.WriteLine("ESSID: " + s.b.essid + " & Name: " + s.b.name);
            //    }
            //}
        }

        private static void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);

        }
    }
}