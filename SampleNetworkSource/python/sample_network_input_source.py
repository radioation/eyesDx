import socket
from time import time, sleep
import math

udpAddr = "127.0.0.1"
udpPort = 23222
time = 0

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
counter = 0;

while True:
	msg = "0," + "Some text value," + str(counter) + "," + str(math.cos(time)) + "," + str(math.sin(time)) + "," + str(counter*counter)
	print ("Sending a packet [" + msg + "]")
	sock.sendto(msg.encode(), (udpAddr, udpPort))
	sleep(0.01)
	time += 0.1
	counter = counter + 1
