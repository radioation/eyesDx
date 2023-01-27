#include "native_io_test.h"

inline bool ok(const char* s) { return strcmp(s, "ok") == 0; }

int main(int argc, char** argv)
{
	char msg[512];


	//////////////////////////////////////////////////////////////////////////
	// Connect to the server.
	//////////////////////////////////////////////////////////////////////////

	EdxState state = EdxNativeIo::Connect(EDX_NATIVE_IO_VERSION_KEY, "native_io.1", 0, msg);

	if (!ok(msg))
	{
		printf("Unable to connect to server [%s]. \n", msg);
		return -1;
	}


	//////////////////////////////////////////////////////////////////////////
	// Get the name of the currently running project.
	//////////////////////////////////////////////////////////////////////////

	char projectName[256];
	EdxNativeIo::GetProjectName(state, projectName, msg);
	printf("Project currently loaded: [%s]. \n", projectName);


	//////////////////////////////////////////////////////////////////////////
	// Get a list of bus data.
	//////////////////////////////////////////////////////////////////////////

	auto listData = make_unique<EdxNativeIo::ListBusData>();
	EdxNativeIo::ListBuses(state, listData.get(), msg);

	if (ok(msg))
	{
		printf("\n------------------------------- \n");
		printf("There are %d buses. \n", listData->BusCount);
		for (int i = 0; i < listData->BusCount; i++)
		{
			printf(" [%d] %s (%d elements). \n",
				i, listData->Buses[i].BusName, listData->Buses[i].ElementCount);
		}
		printf("\n------------------------------- \n\n");
	}


	//////////////////////////////////////////////////////////////////////////
	// Get and set time.
	//////////////////////////////////////////////////////////////////////////

	float timeInSec = 0;
	int64_t utcTime = 0L;
	EdxNativeIo::GetCurrentProjectTime(state, &timeInSec, &utcTime, msg);
	printf("Current time is %1.2f sec, UTC: %I64d. \n", timeInSec, utcTime);


	EdxNativeIo::SetCurrentProjectTime(state, 60.0f, msg);


	//////////////////////////////////////////////////////////////////////////
	// Get some data.
	//////////////////////////////////////////////////////////////////////////

	// Create a big storage buffer.
	const int maxLen = 100'000'000; // 100MB
	auto payload = make_unique<unsigned char[]>(maxLen);
	auto timestamps = make_unique<int64_t[]>(3600 * 1000);
	EdxNativeIo::ElementOutput elements[256];

	for (int i = 0; i < listData->BusCount; i++)
	{
		int frameCount = 0;

		int elCnt = EdxNativeIo::GetBusDataAllElements(
			state,                      // State pointer
			listData->Buses[i].BusName, // Name of bus to get
			0, 0,                       // Get data for ALL time
			payload.get(), maxLen,      // Buffer to populate
			elements, 256,              // Elements to populate
			timestamps.get(),           // Timestamps to populate 
			&frameCount,                // The number of frames produced
			msg);                       // Status message

		if (ok(msg))
		{
			printf("Bus [%s] has %d elements, %d frames. \n",
				listData->Buses[i].BusName, elCnt, frameCount);
			printf("  Runs from %I64dms to %I64dms (%I64d total milliseconds). \n",
				timestamps[0], timestamps[size_t(frameCount) - 1],
				timestamps[size_t(frameCount) - 1] - timestamps[0]);

			if (strcmp(listData->Buses[i].BusName, "Subject_1") == 0)
			{
				float* objX = elements[3].Payload.AsFloat;
				float* objY = elements[4].Payload.AsFloat;

				int maxElems = 10;
				if (maxElems > frameCount) {
					maxElems = frameCount; 
				}
				for (int fr = 0; fr < maxElems; fr++)
				{
					printf("%s - X: %1.3f Y: %1.3f \n",
						listData->Buses[i].BusName,
						objX[fr],
						objY[fr]);
				}
			}
		}
	}





	//////////////////////////////////////////////////////////////////////////
	// Disconnect.
	//////////////////////////////////////////////////////////////////////////

	EdxNativeIo::Disconnect(state, msg);
	return 0;
}
