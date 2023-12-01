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
	char cmd[] = "wpa_cli scan -i wlan0";

	OUTPUT output;
	int res = ExecuteCommand(cmd, &output);

	printf("OUTPUT: %s", output.output_string);

	char tmp = *output.output_string;

	if (strcmp(&tmp, "OK"))
	{
		char cmd[] = "wpa_cli scan_results";
		int res = ExecuteCommand(cmd, &output);

		printf("OUTPUT2: %s", output.output_string);
	}

	exit(0);
}