#include <cstdio>
#include <stdlib.h>
#include "../Wifi.Linux.Core/iwlib.h"
#include "../Wifi.Linux.Core/wrapper.h"
#include "../Wifi.Linux.Core/iwconfig.c"

int main(void) {
	wireless_scan_head head;
	wireless_scan* result;
	iwrange range;
	int sock;
	SCAN* scanResult;


	/* Open socket to kernel */
	sock = iw_sockets_open();

	/* Get some metadata to use for scanning */
	if (iw_get_range_info(sock, "wlan0", &range) < 0) {
		printf("Error during iw_get_range_info. Aborting.\n");
		exit(2);
	}

	/* Perform the scan */
	if (iw_scan(sock, "wlan0", range.we_version_compiled, &head) < 0) {
		printf("Error during iw_scan. Aborting.\n");
		exit(2);
	}

	//int t = wrapper(scanResult);
	SCAN localScan;

	int i = 0;

	/* Traverse the results */
	result = head.result;
	while (NULL != result) {
		printf("%s\n", result->b.essid);
		localScan.scans[i] = result;
		i++;
		result = result->next;
	}

	SCAN* pointerScan = &localScan;

	scanResult = &localScan;

	SCAN ss = *scanResult;

	for (int j = 0; j < i; j++)
	{
		printf("ESSID: %s", ss.scans[j]->b.essid);
	}

	// WIP
	//iwconfig_cmd cm = *find_command("essid GDM key _0123456789!");
	//char* essid = ss.scans[0]->b.essid;
	/*char* essid = "iPhone de Guillaume";

	set_essid_info(sock, "wlan0", &essid, 1);*/

	exit(0);
}