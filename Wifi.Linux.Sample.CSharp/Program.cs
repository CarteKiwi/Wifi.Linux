using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Wifi.Linux.Sample.CSharp
{
    internal class Program
    {
        [DllImport("libWifi.Linux.Core.so", EntryPoint = "ProcessCommand2", CallingConvention = CallingConvention.Cdecl)]
        //[return: MarshalAs(UnmanagedType.I4)]
        public static extern int ProcessCommand2(string command, ref MSG_STRUCT readMsg);

        [DllImport("libWifi.Linux.Core.so", EntryPoint = "ProcessCommand", CallingConvention = CallingConvention.Cdecl)]
        //[return: MarshalAs(UnmanagedType.I4)]
        public static extern int ProcessCommand(string command, ref byte[] buffer);


        [DllImport("libWifi.Linux.Core.so", EntryPoint = "wrapper")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int wrapper(ref SCAN scan);


        [DllImport("libWifi.Linux.Core.so", EntryPoint = "iw_sockets_open")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int iw_sockets_open();


        [DllImport("libWifi.Linux.Core.so", EntryPoint = "iw_get_range_info")]
        [return: MarshalAs(UnmanagedType.I4)]
        static extern int iw_get_range_info(int skfd, string ifname, ref iw_range range);

        [StructLayout(LayoutKind.Sequential)]
        public struct SCAN
        {
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 10)]
            public wireless_scan[] scans;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 10)]
            public WIFI[] wifi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct iw_quality
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            byte[] qual;      /* link quality (%retries, SNR,
					   %missed beacons or better...) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            byte[] level;     /* signal level (dBm) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            byte[] noise;     /* noise level (dBm) */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            byte[] updated;   /* Flags to know if updated */
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct iw_range
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal byte[] we_version_compiled;

            //int throughput;
            //int min_nwid; /* Minimal NWID we are able to set */
            //int max_nwid; /* Maximal NWID we are able to set */
            //UInt16 old_num_channels;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            //byte[] old_num_frequency;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            //byte[] event_capa;
            //Int32 sensitivity;
            //iw_quality max_qual;    /* Quality of the link */
            //iw_quality avg_qual;    /* Quality of the link */
            //__u8 num_bitrates;  /* Number of entries in the list */
            //           __s32 bitrate[IW_MAX_BITRATES]; /* list, in bps */
            //           __s32 min_rts;  /* Minimal RTS threshold */
            //           __s32 max_rts;  /* Maximal RTS threshold */
            //           __s32 min_frag; /* Minimal frag threshold */
            //           __s32 max_frag; /* Maximal frag threshold */
            //           __s32 min_pmp;  /* Minimal PM period */
            //           __s32 max_pmp;  /* Maximal PM period */
            //           __s32 min_pmt;  /* Minimal PM timeout */
            //           __s32 max_pmt;  /* Maximal PM timeout */
            //           int pmp_flags;    /* How to decode max/min PM period */
            //           int pmt_flags;    /* How to decode max/min PM timeout */
            //           int pm_capa;  /* What PM options are supported */
            //           int encoding_size[IW_MAX_ENCODING_SIZES]; /* Different token sizes */
            //           byte[] num_encoding_sizes;    /* Number of entry in the list */
            //           byte[] max_encoding_tokens;   /* Max number of tokens */
            //           byte[] encoding_login_index;  /* token index for login token */
            //           int txpower_capa; /* What options are supported */
            //           byte[] num_txpower;   /* Number of entries in the list */
            //           __s32 txpower[IW_MAX_TXPOWER];  /* list, in bps */
            //           byte[] we_version_compiled;   /* Must be WIRELESS_EXT */
            //           __byte[] u8 we_version_source; /* Last update of source */
            //           int retry_capa;   /* What retry options are supported */
            //           int retry_flags;  /* How to decode max/min retry limit */
            //           int r_time_flags; /* How to decode max/min retry life */
            //           __s32 min_retry;    /* Minimal number of retries */
            //           __s32 max_retry;    /* Maximal number of retries */
            //           __s32 min_r_time;   /* Minimal retry lifetime */
            //           __s32 max_r_time;   /* Maximal retry lifetime */
            //           int num_channels; /* Number of channels [0; num - 1] */
            //           byte[] num_frequency; /* Number of entry in the list */
            //           struct iw_freq  freq[IW_MAX_FREQUENCIES];	/* list */
            //__u32 enc_capa; /* IW_ENC_CAPA_* bit field */
            //           __s32 min_pms;  /* Minimal PM saving */
            //           __s32 max_pms;  /* Maximal PM saving */
            //           int pms_flags;    /* How to decode max/min PM saving */
            //           __s32 modul_capa;   /* IW_MODUL_* bit field */
            //           __u32 bitrate_capa;	/* Types of bitrates supported */



        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG_STRUCT
        {
            int dataSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4128)]
            public byte[] data;

            //public byte[] GetData()
            //{
            //    var bytes = new byte[dataSize];
            //    Marshal.Copy(data, bytes, 0, dataSize);
            //    return bytes;
            //}
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

        static void Main(string[] args)
        {
            Console.WriteLine("Press a key to start");
            Console.ReadLine();

            SCAN scan = new SCAN();
            //wireless_scan[] scan = new wireless_scan[10];
            var w = wrapper(ref scan);
            Console.WriteLine("Wrapper called : " + w);

            if (w == 3)
            {
                foreach (var s in scan.scans)
                {
                    Console.WriteLine("ESSID: " + s.b.essid + " & Name: " + s.b.name);
                }
            }

            //foreach (var s in scan.wifi)
            //{
            //    if (s.essid != null)
            //    {
            //        Console.WriteLine("ESSID: " + new string(s.essid));
            //    }

            //}
            //Console.WriteLine("CALLING 1...");
            //byte[] buffer = new byte[50000];
            //var ok = ProcessCommand("sudo iwlist wlan0 scanning | grep ESSID", ref buffer);

            //if (ok == 1)
            //{
            //    Console.WriteLine(ok);
            //    Console.WriteLine(buffer.Length);
            //    string result = System.Text.Encoding.UTF8.GetString(buffer);
            //    Console.WriteLine(result);
            //}


            //Console.WriteLine("CALLING...");
            //MSG_STRUCT msg = new MSG_STRUCT();
            //var ok2 = ProcessCommand2("sudo iwlist wlan0 scanning", ref msg);

            //if (ok2 == 1)
            //{
            //    Console.WriteLine(ok2);
            //    Console.WriteLine(msg.data);
            //}

            //Console.ReadLine();


            //iw_range range;// = new iw_range();
            //range.we_version_compiled = new byte[16];

            //var sock = iw_sockets_open();

            ////Get some metadata to use for scanning 
            //if (iw_get_range_info(sock, "wlan0", ref range) < 0)
            //{
            //    Console.WriteLine("Error during iw_get_range_info. Aborting.\n");
            //    Console.ReadLine();
            //}

        }
    }
}
