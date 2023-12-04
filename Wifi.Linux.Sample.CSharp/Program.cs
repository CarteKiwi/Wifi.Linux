namespace Wifi.Linux.Sample.CSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                Wpa wpa = new Wpa("wlan0");

                var wifis = await wpa.ScanAsync();

                var selectedWifi = wifis.FirstOrDefault(e => e.Ssid.Contains("Guillaume"));

                if (selectedWifi != null)
                {
                    if (await wpa.SetAsync(selectedWifi.Ssid))
                    {
                        // Ask for Password
                        if (await wpa.TryEnableAsync("0123456789"))
                        {
                            await wpa.SaveAndRebootAsync();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Wifi not found");
                }

            }).Wait();
        }
    }
}