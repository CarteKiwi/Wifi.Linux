using System.Collections;
using System.Runtime.InteropServices;

namespace Wifi.Linux.Sample.CSharp
{
    public class Wifi
    {
        public required string BSSID { get; set; }
        public required string SSID { get; set; }
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

        static void Main(string[] args)
        {
            Console.WriteLine("Press a key to start");
            Console.ReadLine();

            OUTPUT output = new OUTPUT();
            ExecuteCommand("wpa_cli scan -i wlan0", ref output);
            Console.WriteLine("Execute called : " + output.output_string);

            ExecuteCommand("wpa_cli scan_results -i wlan0", ref output);
            Console.WriteLine("Execute called : " + output.output_string);


            var lines = output.output_string.Split('\n');

            string[] titles = new string[4];
            string[][] wifiLines = new string[20][];

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
                //ExecuteCommand("sudo chmod +x /etc/wpa_supplicant.conf", ref output);
                //Console.WriteLine("Execute called : " + output.output_string);

                //ExecuteCommand($"sudo wpa_passphrase {selectedWifi} mypassword > /etc/wpa_supplicant.conf", ref output);
                //Console.WriteLine("Execute called : " + output.output_string);

                ExecuteCommand("wpa_cli add_network -i wlan0", ref output);
                Console.WriteLine("Execute called : " + output.output_string);

                ExecuteCommand($"wpa_cli set_network {output.output_string.Replace("\n", "")} ssid '\"{selectedWifi.SSID}\"' -i wlan0", ref output);
                Console.WriteLine("Execute called : " + output.output_string);
            }
            else
            {
                Console.WriteLine("Not found");
            }

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
    }
}