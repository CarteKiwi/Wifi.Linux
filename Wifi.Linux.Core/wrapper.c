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

int createChild(const char* szCommand, char* const aArguments[], char* const aEnvironment[], const char* szMessage) {
	int aStdinPipe[2];
	int aStdoutPipe[2];
	int nChild;
	char nChar;
	int nResult;

	if (pipe(aStdinPipe) < 0) {
		perror("allocating pipe for child input redirect");
		return -1;
	}
	if (pipe(aStdoutPipe) < 0) {
		close(aStdinPipe[PIPE_READ]);
		close(aStdinPipe[PIPE_WRITE]);
		perror("allocating pipe for child output redirect");
		return -1;
	}

	nChild = fork();
	if (0 == nChild) {
		// child continues here

		// redirect stdin
		if (dup2(aStdinPipe[PIPE_READ], STDIN_FILENO) == -1) {
			exit(errno);
		}

		// redirect stdout
		if (dup2(aStdoutPipe[PIPE_WRITE], STDOUT_FILENO) == -1) {
			exit(errno);
		}

		// redirect stderr
		if (dup2(aStdoutPipe[PIPE_WRITE], STDERR_FILENO) == -1) {
			exit(errno);
		}

		// all these are for use by parent only
		close(aStdinPipe[PIPE_READ]);
		close(aStdinPipe[PIPE_WRITE]);
		close(aStdoutPipe[PIPE_READ]);
		close(aStdoutPipe[PIPE_WRITE]);

		// run child process image
		// replace this with any exec* function find easier to use ("man exec")
		nResult = execve(szCommand, aArguments, aEnvironment);

		// if we get here at all, an error occurred, but we are in the child
		// process, so just exit
		exit(nResult);
	}
	else if (nChild > 0) {
		// parent continues here

		// close unused file descriptors, these are for child only
		close(aStdinPipe[PIPE_READ]);
		close(aStdoutPipe[PIPE_WRITE]);

		while (1) {
			int nRead = 0;

			// Include error check here
			if (NULL != szMessage) {
				printf("\nsending hello message...\n");
				write(aStdinPipe[PIPE_WRITE], szMessage, strlen(szMessage));
			}

			// Just a char by char read here, you can change it accordingly
			while (read(aStdoutPipe[PIPE_READ], &nChar, 1) == 1) {
				nRead++;
				write(STDOUT_FILENO, &nChar, 1);

				if (nRead >= strlen(szMessage)) {
					// read the entire echo of the message, send another one
					break;
				}
			}
			sleep(1);
		}

		// done with these in this example program, you would normally keep these
		// open of course as long as you want to talk to the child
		close(aStdinPipe[PIPE_WRITE]);
		close(aStdoutPipe[PIPE_READ]);
	}
	else {
		// failed to create child
		close(aStdinPipe[PIPE_READ]);
		close(aStdinPipe[PIPE_WRITE]);
		close(aStdoutPipe[PIPE_READ]);
		close(aStdoutPipe[PIPE_WRITE]);
	}
	return nChild;
}

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
	*argc = 0;
	do
	{
		if (*string && (*string != delimiter))
		{
			argv[(*argc)++] = string;
		}
		while (*string && *string != delimiter) string++;
		if (*string) *string++ = 0;
		
	} while (*string);
	return argv;
}

char** extract_tokens(const char* command, int* num_tokens)
{
	char text[256];
	strcpy(text, command);

	//char** tokens = NULL;
	//char* token;
	//int count = 0;

	//token = strtok(text, " ,.");

	//while (token != NULL) {
	//	tokens = realloc(tokens, (count + 1) * sizeof(char*));

	//	//strcpy(tokens[count], token);
	//	tokens[count] = strdup(token);
	//
	//	printf("%s\n", token);

	//	count++;
	//	token = strtok(NULL, " ,.");
	//}

	//tokens[count + 1] = NULL;
	//*num_tokens = count + 1;
	//return tokens;



	char* copy[10];
	char* pch = strtok(text, " ,.");

	int i = 0;
	while (pch != NULL)
	{
		printf("%s\n", pch);
		strcpy(copy[i], pch);
		pch = strtok(NULL, " ,.");
		i++;
	}
	*num_tokens = i;
	return copy;
	/*char* args[i];
	for (int j = 0; j < i; j++)
	{
		args[j] = copy[j];
	}

	*num_tokens = i;

	return args;*/
}

char* qx(char** cmd, int inc_stderr) {
	int stdout_fds[2];
	pipe(stdout_fds);

	int stderr_fds[2];
	if (!inc_stderr) {
		pipe(stderr_fds);
	}

	const pid_t pid = fork();
	if (!pid) {
		close(stdout_fds[0]);
		dup2(stdout_fds[1], 1);
		if (inc_stderr) {
			dup2(stdout_fds[1], 2);
		}

		close(stdout_fds[1]);

		if (!inc_stderr) {
			close(stderr_fds[0]);
			dup2(stderr_fds[1], 2);
			close(stderr_fds[1]);
		}

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

	if (!inc_stderr) {
		close(stderr_fds[1]);
		do {
			const ssize_t r = read(stderr_fds[0], &out[i], buf_size);

			if (r > 0) {
				i += r;
			}

			if (out_size - i <= 4096) {
				out_size *= 2;
				out = realloc(out, out_size);
			}

		} while (errno == EAGAIN || errno == EINTR);

		close(stderr_fds[0]);
	}

	int r, status;
	do {
		r = waitpid(pid, &status, 0);
	} while (r == -1 && errno == EINTR);

	out[i] = 0;

	return out;
}

int ProcessCommand(const char* command, OUTPUT* output)
{
	printf("Processing command...\n");
	printf("%s\n", command);

	char* tokens[20];

	int num_tokens;
	split(tokens, &num_tokens, command, ' ');
	//char** tokens = extract_tokens(command, &num_tokens);

	if (tokens != NULL) {
		char* argv[num_tokens];
		printf("Number of tokens: %d\n", num_tokens);
		for (int i = 0; i < num_tokens; i++) {
			printf("Token %d: %s\n", i + 1, tokens[i]);
			argv[i] = tokens[i];
			//free(tokens[i]);  // Free memory for each token
		}

		/*	char* argv[4];
			argv[0] = "wpa_cli";
			argv[1] = "scan";
			argv[2] = "-i";
			argv[3] = "wlan0";*/

		char* out = qx(argv, 0);
		printf("%s", out);
		output->out = out;
		free(out);
		//free(tokens); // Free the array of tokens
	}




	//if (strlen(out) == strlen("OK\n"))
	//{
	//	free(out);
	//	char* argv[3];
	//	argv[0] = "wpa_cli";
	//	argv[1] = "scan_result";
	//	argv[2] = NULL;
	//	char* out = execute(argv, 0);
	//	printf("%s", out);
	//}

	//free(out);

	return 0;
}

