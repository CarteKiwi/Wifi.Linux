#include <cstdio>
#include <stdlib.h>
#include "../Wifi.Linux.Core/iwlib.h"
#include "../Wifi.Linux.Core/wrapper.h"
#include "../Wifi.Linux.Core/iwconfig.c"

typedef struct {

	int a;
	int b;
	int c;
} my_struct;


int main(void) {
	wireless_scan_head head;
	wireless_scan* result;
	iwrange range;
	int sock;
	SCAN* result2;


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


	/*my_struct* first;
	first->a = 100; first->b = 101; first->c = 1000;

	my_struct* bb;

	printf("first %d %d %d\n", first->a, first->b, first->c);
	bb = first;
	printf("second %d %d %d\n", bb->a, first->b, bb->c);


	first->a = 55; first->b = 55; first->c = 89;
	printf("second %d %d %d\n", bb->a, first->b, bb->c);*/


	//int t = wrapper(result2);
	SCAN localScan;

	int i = 0;

	/* Traverse the results */
	result = head.result;
	while (NULL != result) {
		printf("%s\n", result->b.essid);
		
		/*	WIFI wifi;
			strcpy(wifi.essid, result->b.essid);*/

			//*result2->scans[i].b = result->b;
			//result2->scans[i] = *result;

		/*	localScan.wifis[i] = wifi;
			localScan.scans[i] = result;*/

			//WIFI wifi;
			//strcpy(wifi.essid, result->b.essid);
			//localScan.wifis[i] = wifi;
		localScan.scans[i] = result;

		//memcpy(result2->scans[i], result, sizeof(result));
		i++;
		//result2->scans[i].b = result->b;
		result = result->next;
	}

	SCAN* pointerScan = &localScan;

	result2 = &localScan;

	SCAN ss = *result2;

	for (int j = 0; j < i; j++)
	{
		//printf("wifis %s\n", *result2->wifis[j]->essid);
		//printf("scans %s\n", *result2->scans[j]->b.essid);
		//printf("ESSID: " + localScan->wifis[j].essid);
		//printf("ESSID: " + localScan.scans[j]->b->essid);
		printf("ESSID: %s", ss.scans[j]->b.essid);
	}

	//iwconfig_cmd cm = *find_command("essid GDM key _0123456789!");
	//char* essid = ss.scans[0]->b.essid;
	char* essid = "iPhone de Guillaume";

	set_essid_info(sock, "wlan0", &essid, 1);

	exit(0);
}