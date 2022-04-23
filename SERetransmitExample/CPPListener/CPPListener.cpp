// CPPListener.cpp : Defines the entry point for the console application.
//

#include <iostream>
#include <string>

#ifdef _WIN32
#include "stdafx.h"
#include <winsock2.h>
#include <WS2tcpip.h>
#pragma comment(lib,"ws2_32.lib") // link against Winsock Library
#else
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdlib.h>
#include <string.h>
#include <inttypes.h> 
#include <unistd.h>
#endif

#define SERVER "127.0.0.1"  
#define BUF_LEN 2048  
#define PORT 51500

int main()
{
	struct sockaddr_in address;
	int s = 0;
	socklen_t alen = sizeof(address);
	char buf[BUF_LEN];


#ifdef _WIN32
	WSADATA wsa;


	// startup Winsock
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		std::cerr << "WSAStartup() failed" << std::endl;
		return -1;
	}
#endif

	// Create the UDP socket
	if ((s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) < 0)
	{
		std::cerr << "socket() failed" << std::endl;
		return -2;
	}

	//setup address structure
	memset((char *)&address, 0, sizeof(address));
	address.sin_family = AF_INET;
	address.sin_port = htons(PORT);
#ifdef _WIN32 
	InetPton(AF_INET, _T(SERVER), &address.sin_addr.s_addr);
#else
	inet_pton(AF_INET, SERVER, &address.sin_addr.s_addr);
#endif


	if (bind(s, (struct sockaddr *)&address, sizeof(address)) < 0)
	{
		std::cerr << "bind() failed" << std::endl;
		return -3;

	} 
	
	for( int i=0; i < 9000; ++i )
	{
		// wait for message from adapter
		memset(buf, 0, BUF_LEN);
		if (recvfrom(s, buf, BUF_LEN, 0, (struct sockaddr *) &address, &alen) < 0)
		{
			std::cerr << "recvfrom() failed" << std::endl;
			return -4;
		}

		// do conversions
		float x, y;
		int64_t timestamp = 0;
		memcpy(&timestamp, buf, sizeof(int64_t));
		int offset = sizeof(int64_t);
		memcpy(&x, buf + offset, 4);
		offset += 4;
		memcpy(&y, buf + offset, 4);
		offset += 4;
		std::string name = buf + offset;
		offset += 64;
		float lopen, ropen, eopen;
		memcpy(&lopen, buf + offset, 4);
		offset += 4;
		memcpy(&ropen, buf + offset, 4);
		offset += 4;
		memcpy(&eopen, buf + offset, 4);
		offset += 4;
		std::cout << "Filetime: " << timestamp << " X: " << x << " y: " << y  << " objectName: " << name << std::endl;
		std::cout << "left eyelid opening:  " << lopen << " Right eyelid opening: " << ropen << " eyelid opening: " << eopen << std::endl;
		
	}

#ifdef _WIN32
	closesocket(s);

	WSACleanup();
#else
	close(s);
#endif

	return 0;
}

