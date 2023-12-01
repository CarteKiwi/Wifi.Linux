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

	printf("OUTPUT: %s", output.output_string);

	char tmp = *output.output_string;

	if (strcmp(&tmp, "OK"))
	{
		char cmd[] = "wpa_cli scan_results";
		int res = ProcessCommand(cmd, &output);

		printf("OUTPUT2: %s", output.output_string);
	}

	exit(0);
}