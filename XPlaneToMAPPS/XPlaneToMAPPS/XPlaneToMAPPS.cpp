#include <iostream>
#include <fstream>
#include <sstream>


#include <winsock2.h>
#include <WS2tcpip.h>
#pragma comment(lib,"ws2_32.lib") // link against Winsock Library

#include <string.h>

#include "XPLMDisplay.h"
#include "XPLMGraphics.h"
#include "XPLMProcessing.h"
#include "XPLMDataAccess.h"
#include "XPLMUtilities.h"


#if IBM
#include <windows.h>


#endif
#if LIN
#include <GL/gl.h>
#elif __GNUC__
#include <OpenGL/gl.h>
#else
#include <GL/gl.h>
#endif

#ifndef XPLM300
#error This is made to be compiled against the XPLM300 SDK
#endif

// References to flight data
static XPLMDataRef  planeLatitude;
static XPLMDataRef  planeLongitude;
static XPLMDataRef  planeElevation;
static XPLMDataRef  planeGroundspeed;
static XPLMDataRef  planeTrueAirspeed;

// Callback to send data to MAPPS
static float	MAPPSCallback(
	float                inElapsedSinceLastCall,
	float                inElapsedTimeSinceLastFlightLoop,
	int                  inCounter,
	void *               inRefcon);


// eyesDx 
#define SERVER "127.0.0.1"  
#define BUF_LEN 2048  
#define PORT 51500
WSADATA wsaData;
SOCKET sock = INVALID_SOCKET;
sockaddr_in address;

// Called when the plugin is first loaded.  
// Use to allocate permanent resource and register any
// other callbacks you need
PLUGIN_API int XPluginStart(
	char *		outName,
	char *		outSig,
	char *		outDesc) {
	strcpy(outName, "MAPPS");
	strcpy(outSig, "eyesdx.examples.mapps");
	strcpy(outDesc, "A simple plugin for use with MAPPS");


	// startup Winsock
	auto result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (result != NO_ERROR)
	{
		std::cerr << "WSAStartup() failed" << std::endl;
		return 0;
	}


	// Create the UDP socket
	sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (sock == INVALID_SOCKET) {
		std::cerr << "socket() failed: " << WSAGetLastError() << std::endl; 
		WSACleanup();
		return 0;
	}

	//setup address structure
	memset((char *)&address, 0, sizeof(address));
	address.sin_family = AF_INET;
	address.sin_port = htons(PORT);
	InetPton(AF_INET, SERVER, &address.sin_addr.s_addr);


	//  Find the data refs by name
	planeLatitude = XPLMFindDataRef("sim/flightmodel/position/latitude");
	planeLongitude = XPLMFindDataRef("sim/flightmodel/position/longitude");
	planeElevation = XPLMFindDataRef("sim/flightmodel/position/elevation");
	planeGroundspeed = XPLMFindDataRef("sim/flightmodel/position/groundspeed");
	planeTrueAirspeed = XPLMFindDataRef("sim/flightmodel/position/true_airspeed");


	// register callback 
	XPLMRegisterFlightLoopCallback(
		MAPPSCallback,
		1.0,				// calling interval
		NULL);

	return 1;
}

// called when the plugin is enabled.  You don't
// need to do anything in this callback
PLUGIN_API int  XPluginEnable(void) {
	return 1;
}

// called when the plugin is disabled.  You don't 
// need to do anythig in this plugin
PLUGIN_API void XPluginDisable(void) {
}

// called right before the plugin is unloaded. Do
// cleanup here ( unrigister callbacks, release resources,
// close files, etc )
PLUGIN_API void	XPluginStop(void) {

	XPLMUnregisterFlightLoopCallback(MAPPSCallback, NULL);

	closesocket(sock);

	WSACleanup();
}

// called when a plugin or X-Plane sends the pulgin a message
PLUGIN_API void XPluginReceiveMessage(XPLMPluginID inFrom, int inMsg, void * inParam) {
}


float	MAPPSCallback(
	float                inElapsedSinceLastCall,
	float                inElapsedTimeSinceLastFlightLoop,
	int                  inCounter,
	void *               inRefcon)
{
	// read out data 
	double	latitude = XPLMGetDatad(planeLatitude);
	double	longitude = XPLMGetDatad(planeLongitude);
	double	elevation = XPLMGetDatad(planeElevation);
	float	groundspeed = XPLMGetDataf(planeGroundspeed);
	float	trueAirspeed = XPLMGetDataf(planeTrueAirspeed);

	std::stringstream ss;
	ss << "0," << latitude << "," << longitude << "," << elevation << "," << groundspeed << "," << trueAirspeed << "\n";
	auto result = sendto(sock, ss.str().c_str(), ss.str().length(), 0, (sockaddr*)&address, sizeof(address));
	// Return 1.0 to indicate that we want to be called again in another second.
	return 1.0;
}