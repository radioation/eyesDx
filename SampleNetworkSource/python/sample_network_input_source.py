import socket
from time import time, sleep
import datetime
import math

udpAddr = "127.0.0.1"
udpPort = 23222

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
counter = 0
elapsed = 0

while True:
    # make FileTime from current time.
    current_time = time()
    current_time_ms = int( current_time * 1000)
    unix_epoch_start_in_filetime = 11644473600000
    filetime = (unix_epoch_start_in_filetime + current_time_ms) * 10000  # Convert to 100s of nanoseconds

    # make mesage
    msg = str(filetime) + "," + "Some text value," + str(counter) + "," + str(math.cos(elapsed)) + "," + str(math.sin(elapsed)) + "," + str(counter*counter)
    print ("Sending a packet [" + msg + "]")
    sock.sendto(msg.encode(), (udpAddr, udpPort))
    sleep(0.01)
    elapsed += 0.1
    counter = counter + 1
