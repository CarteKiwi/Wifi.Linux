#include <stdlib.h>
#include "../Wifi.Linux.Core/iwlib.h"
#include "../Wifi.Linux.Core/wrapper.h"

int wrapper(SCAN* scan) {

	wireless_scan_head head;
	wireless_scan* result;
	iwrange range;
	int sock;

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

	scan = &localScan;

	SCAN ss = *scan;

	for (int j = 0; j < i; j++)
	{
		printf("ESSID: %s", ss.scans[j]->b.essid);
	}

	return 3;
};

int ProcessCommand(const char* command, char** buffer[50000])
{
	int org_stdout = dup(1);
	int org_stderr = dup(2);

	printf("ProcessCommand...\n");
	printf("%s\n", command);

	int pd[2];
	printf("Before pipe\n");
	pipe(pd);

	// Make the read-end of the pipe non blocking, so if the command being
	// executed has no output the read() call won't get stuck
	int flags = fcntl(pd[0], F_GETFL);
	flags |= O_NONBLOCK;

	if (fcntl(pd[0], F_SETFL, flags) == -1) {
	}

	// Redirect stdout and stderr to the write-end of the pipe
	printf("Before dup21\n");
	dup2(pd[1], 1);
	printf("Before dup2\n");
	dup2(pd[1], 2);
	printf("Before system\n");
	int status = system(command);
	int buf_size = 50000;
	char buf[buf_size];

	// Read from read-end of the pipe
	long num_bytes = read(pd[0], buf, buf_size);

	if (num_bytes > 0) {
		buffer = buf;
	}

	return 1;
}


int ProcessCommand2(const char* command, msg* buffer)
{
	printf("ProcessCommand...");

	FILE* fp = NULL;
	char* ret = NULL;
	size_t len = 0;
	fp = popen(command, "r");
	if (getline(&ret, &len, fp) < 0) {
		fprintf(stderr, "whole line can not be read\n");
		strcpy(ret, "");
	}
	if (!strcmp(ret, "")) {
		strcpy(*buffer->data, "");
	}
	else {
		size_t len = strlen(ret);
		if (ret[len - 1] == '\n')
			ret[--len] = '\0';
		strcpy(*buffer->data, ret);
	}
	free(ret);
	pclose(fp);

	return 1;
}