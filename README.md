# eyesDx
Examples for communicating with [EyesDx](http://www.eyesdx.com) software

## SampleNetworkSource
This program demonstrates sending to the eyesDx network adapter with C#.

## SampleEventSource
This program demonstrates sending events to the eyesDx network adapter with C#.

## SERetransmitExample
This example provides C# and C++ programs that listen to data transmitted from the eyesDx adapters.

To use this example

1.  Copy retransmit.xml to the the SmartEye Adapter configuration folder C:\eDx_data\configuration\Adapter Launcher 2018.1\SmartEye Adapter

2.  Start SmartEye Pro. It must be configured to send data to the eyesDx SmartEye Adapter

3.  Launch "Adapter Launcher 2018" 

4.  In Adapter Launcher, set the "Adapter Type:" to "Smarteye Adapter" and set the "Settings file:" to"retransmit.xml" 

5. Press "Launch Now" to start the adapter.

4. Build and run either "CPPListner" or "SERetransmitExample" on the same machine as the SmartEye Adapter.   


### Retransmission Configuration

The retransmit 



### Linux
The CPPListener.cpp file has been compiled and tested on Ubuntu Linux 16.04.1 with g++ version 5.4.0.
