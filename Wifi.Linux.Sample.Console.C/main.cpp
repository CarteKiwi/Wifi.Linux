#include <cstdio>
#include <stdlib.h>
#include <string>
#include <wait.h>

extern "C" {
#include "../Wifi.Linux.Core/iwlib.h"
#include "../Wifi.Linux.Core/wrapper.h"
}



int main(void) {
	/*SCAN* scanResult;

	int t = ScanWifis(scanResult);

	for (int j = 0; j < sizeof(scanResult->scans); j++)
	{
		printf("ESSID: %s", scanResult->scans[j]->b.essid);
	}*/

	OUTPUT output;
	char cmd[] = "wpa_cli scan -i wlan0";
	int res = ProcessCommand(cmd, &output);

	printf("OUTPUT: %s", output.out);

	//// WIP
	////iwconfig_cmd cm = *find_command("essid GDM key _0123456789!");
	////char* essid = ss.scans[0]->b.essid;
	///*char* essid = "iPhone de Guillaume";

	//set_essid_info(sock, "wlan0", &essid, 1);*/

	exit(0);
}