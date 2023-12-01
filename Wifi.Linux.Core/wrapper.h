typedef struct {
	wireless_scan* scans[10];
} SCAN;

int ScanWifis(SCAN* scan);

typedef struct {
	unsigned char* output_string;
} OUTPUT;

int ExecuteCommand(const char* command, OUTPUT* output);


char* qx(char** cmd);
