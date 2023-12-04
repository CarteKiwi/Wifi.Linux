using System.Diagnostics;
using System.Text;

namespace Wifi.Linux
{
    public class Wpa
    {
        private string sblock = "";
        private string _ifname;
        private int _networkIndex;

        public Wpa(string ifname)
        {
            _ifname = ifname;
        }

        public async Task<Wifi[]> ScanAsync()
        {
            // Scan wifis
            var output = await Exec("wpa_cli", $"scan -i {_ifname}");
            Console.WriteLine(output);

            // Get scan results
            output = await Exec("wpa_cli", $"scan_results -i {_ifname}");

            return OutputToWifi(output).ToArray();
        }

        public async Task<bool> SetAsync(string ssid)
        {
            var output = await Exec("wpa_cli", "remove_network all");
            Console.WriteLine("Execute called : " + output);

            output = await Exec("wpa_cli", "add_network");
            Console.WriteLine("Execute called : " + output);

            _networkIndex = int.Parse(output.Split("\n")[1].Replace("\n", ""));

            output = await Exec($"/bin/bash", $"-c \"wpa_cli set_network {_networkIndex} ssid '\\\"{ssid}\\\"'\"");
            Console.WriteLine("Execute called : " + output);

            return true;
        }

        public async Task<bool> TryEnableAsync(string pwd)
        {
            try
            {
                var output = await Exec($"/bin/bash", $"-c \"wpa_cli set_network {_networkIndex} psk '\\\"{pwd}\\\"'\"");
                Console.WriteLine("Execute called : " + output);

                output = await Exec("wpa_cli", $"enable_network {_networkIndex}");
                Console.WriteLine("Execute called : " + output);

                output = await Exec("wpa_cli", $"select_network {_networkIndex}");
                Console.WriteLine("Execute called : " + output);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task SaveAndRebootAsync()
        {
            var output = await Exec("wpa_cli", $"save_config");
            Console.WriteLine("Execute called : " + output);

            output = await Exec("wpa_cli", $"reconfigure");
            Console.WriteLine("Execute called : " + output);

            output = await Exec("reboot", $"");
            Console.WriteLine("Execute called : " + output);
        }

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

        private IEnumerable<Wifi> OutputToWifi(string output)
        {
            var lines = output.Split('\n');

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

            return wifis;
        }
    }
}