typedef struct {
	wireless_scan* scans[10];
} SCAN;

int ScanWifis(SCAN* scan);




typedef struct {
	unsigned char* out;
} OUTPUT;

int ProcessCommand(const char* command, OUTPUT* output);


char* qx(char** cmd, int inc_stderr);
