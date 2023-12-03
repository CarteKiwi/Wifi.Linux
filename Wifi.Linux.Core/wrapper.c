#include <stdlib.h>
#include "../Wifi.Linux.Core/iwlib.h"
#include "../Wifi.Linux.Core/wrapper.h"

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>

#include <string.h>

#define PIPE_READ 0
#define PIPE_WRITE 1

int ScanWifis(SCAN* scan) {

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

	return 1;
};

char** split(char** argv, int* argc, char* string, const char delimiter)
{
	const char quote = '"';
	int hasQuote = 0;

	*argc = 0;
	do
	{

		/*if (*string && (*string != delimiter))
		{*/
		argv[(*argc)++] = string;
		//}
		if (*string && *string == delimiter)
		{
			string++;

		}

		while (*string && *string != delimiter || hasQuote == 1)
		{
			if (*string == quote)
				hasQuote = 1 - hasQuote;

			string++;
		}

		if (*string)
		{
			if (*string && *string == delimiter)
				string--;
			*string++ = 0;
		}

	} while (*string);
	return argv;
}

char* qx(char** cmd) {
	int stdout_fds[2];
	pipe(stdout_fds);

	const pid_t pid = fork();

	if (!pid) {
		close(stdout_fds[0]);
		dup2(stdout_fds[1], 1);
		close(stdout_fds[1]);

		execvp(*cmd, cmd);

		exit(0);
	}

	close(stdout_fds[1]);

	const int buf_size = 4096;
	char* out = malloc(buf_size);
	int out_size = buf_size;
	int i = 0;
	do {
		const ssize_t r = read(stdout_fds[0], &out[i], buf_size);
		if (r > 0) {
			i += r;
		}

		if (out_size - i <= 4096) {
			out_size *= 2;
			out = realloc(out, out_size);
		}
	} while (errno == EAGAIN || errno == EINTR);

	close(stdout_fds[0]);

	int r, status;
	do {
		r = waitpid(pid, &status, 0);
	} while (r == -1 && errno == EINTR);

	out[i] = 0;

	return out;
}

int ExecuteCommand(const char* command, OUTPUT* output)
{
	printf("Processing command...\n");
	printf("%s\n", command);

	char* tokens[20];

	int num_tokens;
	split(tokens, &num_tokens, command, '-');

	if (tokens != NULL) {
		char* argv[num_tokens + 1];
		printf("Number of tokens: %d\n", num_tokens);
		for (int i = 0; i < num_tokens; i++) {
			printf("Token %d: %s\n", i + 1, tokens[i]);
			argv[i] = tokens[i];
		}

		argv[num_tokens + 1] = NULL;

		char* out = qx(argv);
		printf("output: %s", out);
		output->output_string = out;
		//free(out);
	}

	return 0;
}

