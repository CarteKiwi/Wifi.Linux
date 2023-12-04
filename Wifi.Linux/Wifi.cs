namespace Wifi.Linux
{
    public class Wifi
    {
        public required string Bssid { get; set; }
        public string? Frequency { get; set; }
        public string? Signal { get; set; }
        public string? Flags { get; set; }
        public required string Ssid { get; set; }
    }
}
