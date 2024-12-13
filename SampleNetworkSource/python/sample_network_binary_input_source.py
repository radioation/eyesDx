import socket
import struct   # Use `struct` to pack values into a binary format.
from time import time, sleep
import math

import datetime

# Multicast address and port
udpAddr = "127.0.0.1"
udpPort = 23223

# Define the blob's structure
#  ( see https://docs.python.org/3/library/struct.html )
#  @Q  : Native unsigned long long (for FILETIME)
#  s64 : 64 byte string
#  i   : Integer
#  f   : Float
#  d   : Double
#  @q  : long long 
BLOB_FORMAT = '@Q64sifdqiii'

# Blob data to send
text_string = "Test data string"
int_value = 0 
float_value = 1.0
double_value = 64000.0
int64_value = 133786028370000000
# 3 integers
int_values = [1,2,3] 

elapsed=0.0

# Function to get the current FILETIME timestamp
def get_current_filetime():
     current_time = time()
     current_time_ms = int( current_time * 1000)
     unix_epoch_start_in_filetime = 11644473600000
     filetime = (unix_epoch_start_in_filetime + current_time_ms) * 10000  # Convert to 100s of nanoseconds
     return filetime


# Set up a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)

# Send the blob repeatedly
try:
    while True:
        # get current time stamp 
        filetime_timestamp = get_current_filetime()


        # Pack the message into a binary format
        binary_blob = struct.pack(BLOB_FORMAT, filetime_timestamp, text_string.encode('utf-8')[:63].ljust(63, b'\0'), int_value, float_value, double_value, int64_value, *int_values)
        print( "sending binary blob")
        sock.sendto(binary_blob, (udpAddr, udpPort))

        sleep(0.02)  # Adjust frequency as needed

        # update values for next send
        elapsed += 0.1 
        int_value = int_value + 1
        float_value = math.cos(elapsed)
        double_value = 64000.0 * math.sin(elapsed)
        int64_value = int64_value + 1
        temp_int = int_values[0]
        int_values[0] = int_values[1]
        int_values[1] = int_values[2]
        int_values[2] = temp_int



except KeyboardInterrupt:
    print("Stopped sending")
finally:
    sock.close()

