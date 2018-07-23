// CPPListener.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <winsock2.h>
#include <WS2tcpip.h>
#include <iostream>
#pragma comment(lib,"ws2_32.lib") // link against Winsock Library

#define SERVER "127.0.0.1"  
#define BUFLEN 2048  
#define PORT 51500

int main()
{
	struct sockaddr_in address;
	int s, slen = sizeof(address);
	char buf[BUFLEN];
	char message[BUFLEN];
	WSADATA wsa;

	// startup Winsock
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		std::cerr << "WSAStartup() failed with : " << WSAGetLastError() << std::endl;
		exit(EXIT_FAILURE);
	}

	// Create the UDP socket
	if ((s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == SOCKET_ERROR)
	{
		std::cerr << "socket() failed with : " << WSAGetLastError() << std::endl;
		exit(EXIT_FAILURE);
	}

	//setup address structure
	memset((char *)&address, 0, sizeof(address));
	address.sin_family = AF_INET;
	address.sin_port = htons(PORT);
	// address.sin_addr.S_un.S_addr = inet_addr(SERVER);
	InetPton(AF_INET, _T(SERVER), &address.sin_addr.s_addr);


	if (bind(s, (struct sockaddr *)&address, sizeof(address)) == SOCKET_ERROR)
	{
		printf("Bind failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}
	puts("Bind done");
	
	while (1)
	{
		// wait for message from adapter
		memset(buf, '\0', BUFLEN);
		if (recvfrom(s, buf, BUFLEN, 0, (struct sockaddr *) &address, &slen) == SOCKET_ERROR)
		{
			std::cerr << "recvfrom() failed with : " << WSAGetLastError() << std::endl;
			exit(EXIT_FAILURE);
		}
		float x, y;
		INT64 timestamp = 0;
		memcpy(&timestamp, buf, sizeof(INT64));
		int offset = sizeof(INT64);
		memcpy(&x, buf + offset, 4);
		offset += 4;
		memcpy(&y, buf + offset, 4);
		offset += 4;
		std::cout << "Filetime: " << timestamp << " X: " << x << " y: " << y  << " objectName: " << buf + offset << std::endl;
		// do conversions
	}

	closesocket(s);
	WSACleanup();

	return 0;
}

