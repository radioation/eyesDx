// SampleTCPStream.cpp : Defines the entry point for the console application.
//

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>


#include <iostream>
#include <string>
 

#define NOMINMAX  
#include <winsock2.h>
#include <WS2tcpip.h>
#pragma comment(lib,"ws2_32.lib") // link against Winsock Library
 

constexpr auto SERVER = "127.0.0.1";
constexpr auto BUF_LEN = 2048;
constexpr auto PORT = 23224;


// public domain CRC32 from http://home.thep.lu.se/~bjorn/crc/

uint32_t crc32_for_byte(uint32_t r) {
	for (int j = 0; j < 8; ++j)
		r = (r & 1 ? 0 : (uint32_t)0xEDB88320L) ^ r >> 1;
	return r ^ (uint32_t)0xFF000000L;
}

void crc32(const void* data, size_t n_bytes, uint32_t* crc) {
	static uint32_t table[0x100];
	if (!*table)
		for (size_t i = 0; i < 0x100; ++i)
			table[i] = crc32_for_byte(i);
	for (size_t i = 0; i < n_bytes; ++i)
		*crc = table[(uint8_t)*crc ^ ((uint8_t*)data)[i]] ^ *crc >> 8;
}


 // eyesDx Stream Example starts here

int main()
{
	// Setup addresses
	struct sockaddr_in address;
	int s = 0;
	socklen_t alen = sizeof(address);
	char buf[BUF_LEN];


	// startup Winsock
	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		std::cerr << "WSAStartup() failed" << std::endl;
		return -1;
	} 

	// Create the TCP socket
	if ((s = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		std::cerr << "socket() failed" << std::endl;
		return -2;
	}

	// setup address structure
	memset((char*)&address, 0, sizeof(address));
	address.sin_family = AF_INET; 
	address.sin_port = htons(PORT);
 
	InetPton(AF_INET, SERVER, &address.sin_addr.s_addr);
 

	if (connect(s, (struct sockaddr*) &address, sizeof(address)) != 0) {
		std::cerr <<  "connection with the server failed..." << std::endl;
		return -3;
	} 

	float time = 0.0f;
	int counter = 0;
	for ( int i=0; i < 200000; ++i )
	{ 
		// clear buffer.
		memset(buf, 0, BUF_LEN);
		 
		// set headerkey
		int headerKey = 0x1234567;
		memcpy(buf, &headerKey, sizeof(headerKey)); 
		int offset = sizeof(int);

		// Do CRC later.
		offset += sizeof(int);
		 
		SYSTEMTIME systime;
		GetSystemTime(&systime);
		 
		//  get UTC time as Windows Filetime.
		FILETIME filetime;
		SystemTimeToFileTime(&systime, &filetime); 
		int64_t timestamp = (uint64_t)filetime.dwHighDateTime << 32 | (uint64_t)filetime.dwLowDateTime;
		memcpy(buf+offset, &timestamp, sizeof(timestamp));
		offset += sizeof(timestamp);


		// Add 'SomeTextField' to the network adapter message
		std::string message = "Some C++ Text Field";
		memcpy(buf+ offset, message.c_str(), message.length());
		// Defined String64 in confiruration XML. So we need move 64 bytes 
		offset += 64;

		// Add 'AnIntegerField' to the network adapter message
		int someInt = counter;
		memcpy(buf + offset, &someInt, sizeof(someInt));
		offset += sizeof(someInt);

		// Add 'SomeFloatField' to the network adapter message
		float cosAngle = (float)cos(time);
		memcpy(buf + offset, &cosAngle, sizeof(cosAngle));
		offset += sizeof(cosAngle);

		// Add 'SomeDoubleField' to the network adapter message
		double sinAngle = sin(time);
		memcpy(buf + offset, &sinAngle, sizeof(sinAngle));
		offset += sizeof(sinAngle);

		// Add 'Some64BitIntField' to the network adapter message
		int64_t max64 = std::numeric_limits<int64_t>::max() - counter;
		memcpy(buf + offset, &max64, sizeof(max64));
		offset += sizeof(max64);

		// compute CRC hash 
		unsigned int hash = 0;
		crc32(buf + 16, 88, &hash);
		memcpy(buf + 4, &hash, sizeof(hash));

		if (sendto(s, buf, 96 + 8, 0, (struct sockaddr*)&address, sizeof(address)) < 0)
		{
			std::cerr << "sendto() failed: "<< WSAGetLastError() << std::endl;

			return -4;
		}
		Sleep(10);
		time += 0.1f;
		counter += 1;
		if (counter > 2000)
		{
			counter = 0;
		}
 
	}

	closesocket(s);
	WSACleanup();

	return 0;
}

