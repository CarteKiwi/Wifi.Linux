typedef struct {
	char essid[IW_ESSID_MAX_SIZE + 1];
} WIFI;

typedef struct {
	wireless_scan* scans[10];
	WIFI* wifis[10];
} SCAN;


int wrapper(SCAN* scan);

int ProcessCommand(const char* command, char** buffer[50000]);

typedef struct MSG_STRUCT msg;

struct MSG_STRUCT {
	unsigned long dataSize;
	unsigned char* data;
};



int ProcessCommand2(const char* command, msg* buffer);