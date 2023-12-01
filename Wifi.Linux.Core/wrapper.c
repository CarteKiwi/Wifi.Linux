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

char* qx(char** cmd) {
	printf("A");

	int stdout_fds[2];
	pipe(stdout_fds);

	const pid_t pid = fork();

	//if (pid == -1)
	//{
	//	/* Failed to fork - one return */
	//	printf("FAILED");
	//}
	//else if (pid == 0)
	//{
	//	sleep(3);
	//	/* Child process - distinct from original process */
	//	printf("CHILD");

	//	close(stdout_fds[0]);
	//	dup2(stdout_fds[1], 0);
	//	close(stdout_fds[1]);

	//	execvp(*cmd, cmd);
	//	exit(0);
	//}
	//else
	//{
	//	/* Parent process - distinct from child process */
	//	printf("PARENT");
	//}

	if (!pid) {
		sleep(3);
		/* Child process - distinct from original process */
		printf("CHILD");

		close(stdout_fds[0]);
		dup2(stdout_fds[1], 1);
		close(stdout_fds[1]);

		execvp(*cmd, cmd);

		exit(0);
	}

	printf("B");

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
	printf("C");

	out[i] = 0;

	return out;
}


char* exec2(char* cmd)
{
	pid_t         pid;
	int           pipe_fd[2];

	if (pipe(pipe_fd) == -1)
	{
		perror("pipe");
		return 1;
	}

	if ((pid = fork()) == -1)
	{
		perror("fork");
		return 1;
	}
	else if (pid == 0)
	{
		close(pipe_fd[0]);
		if (dup2(pipe_fd[1], 1) == -1)
			perror("dup2");
		else if (execvp(*cmd, cmd) == -1)
			perror("execve");
		return 1;
	}
	else
	{
		char      buffer[1024];
		int       ret;
		int       status;

		close(pipe_fd[1]);

		while ((ret = read(pipe_fd[0], buffer, 1023)) != 0)
		{
			printf("%d\n", ret);
			buffer[ret] = 0;
			printf("%s\n", buffer);
		}
	}

	return 0;
}


char* execute(char* cmd)
{
	const int buf_size = 4096;
	char* out = malloc(buf_size);

	FILE* fp;
	char path[1035];
	char line[1000];

	strcpy(line, cmd);

	/* Open the command for reading. */
	fp = popen(line, "r");
	if (fp == NULL) {
		printf("Failed to run command\n");
		exit(1);
	}

	int i = 0;
	/* Read the output a line at a time - output it. */
	while (fgets(path, sizeof(path), fp)) {
		printf("%s", path);
		out[i] = path;
		i++;
	}

	/* close */
	pclose(fp);

	return out;
}

int ExecuteCommand(const char* command, OUTPUT* output)
{
	printf("Processing command...\n");
	printf("%s\n", command);


	//char* out = execute(command);
	//printf("output: %s", out);
	//output->output_string = out;

	char* tokens[20];

	int num_tokens;
	split(tokens, &num_tokens, command, ' ');

	if (tokens != NULL) {
		char* argv[num_tokens + 1];
		printf("Number of tokens: %d\n", num_tokens);
		for (int i = 0; i < num_tokens; i++) {
			printf("Token %d: %s\n", i + 1, tokens[i]);
			argv[i] = tokens[i];
		}

		argv[num_tokens + 1] = NULL;

		/*	int pipefd[2];
			int status;
			uint8_t buf[256];

			pipe(pipefd);

			int fid = fork();
			if (fid == 0) {
				close(pipefd[0]);
				dup2(pipefd[1], 1);
				close(pipefd[1]);
				execvp(*argv, argv);
			}
			close(pipefd[1]);

			while (read(pipefd[0], buf, 1) > 0)
				write(1, buf, 1);
			wait(&status);
			return 0;*/

			//int status;
			//pid_t child;
			//if (child = fork()) {
			//	waitpid(child, &status, 0);
			//	/*now we know execve is finished*/
			//	if (WIFEXITED(status))
			//		exit(WEXITSTATUS(status));
			//	exit(1);
			//}



		char* out = qx(argv);
		//char* out = execute(command);
		printf("output: %s", out);
		output->output_string = out;
		//free(out);
	}

	return 0;
}

