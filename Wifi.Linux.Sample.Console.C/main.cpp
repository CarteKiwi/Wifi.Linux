#include <cstdio>
#include <stdlib.h>
#include <string>
#include <wait.h>

extern "C" {
#include "../Wifi.Linux.Core/iwlib.h"
#include "../Wifi.Linux.Core/wrapper.h"
}

int main(void) {





	FILE* fp;
	char path[1035];

	/* Open the command for reading. */
	fp = popen("wpa_passphrase \"iPhone de Guillaume 2\" \"0123456789\"", "r");
	if (fp == NULL) {
		printf("Failed to run command\n");
		exit(1);
	}

	/* Read the output a line at a time - output it. */
	while (fgets(path, sizeof(path), fp) != NULL) {
		printf("%s", path);
	}

	/* close */
	pclose(fp);

	return 0;








	/*SCAN* scanResult;

	int t = ScanWifis(scanResult);

	for (int j = 0; j < sizeof(scanResult->scans); j++)
	{
		printf("ESSID: %s", scanResult->scans[j]->b.essid);
	}*/
	char cmd[] = "wpa_passphrase -i \"iPhone de Guillaume\" 0123456789";

	OUTPUT output;
	int res = ExecuteCommand(cmd, &output);

	printf("OUTPUT: %s", output.output_string);

	/*char tmp = *output.output_string;

	if (strcmp(&tmp, "OK"))
	{
		char cmd[] = "wpa_cli scan_results";
		int res = ExecuteCommand(cmd, &output);

		printf("OUTPUT2: %s", output.output_string);
	}*/

	exit(0);
}