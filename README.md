# eyesDx
Examples for communicating with [EyesDx](https://www.eyesdx.com) software

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

The retransmission function can be configured using the `<retransmit>` elements in the adapter configuration file.
```
  <retransmit>
    <address>127.0.0.1</address>
    <port>51500</port>
    <fields>ObjectIntersectionX,ObjectIntersectionY,ObjectIntersectionName</fields>
    <sleep>10</sleep>
    <mode>binary</mode>
  </retransmit>
```
Use `<address>` and `<port>` to control where the adapter sends the data stream.
Use `<fields>` to specify which data fields to retransmit
Use `<sleep>` to control how long the adapter waits (in ms) if no data is present
`<mode>` is a placeholder and not yet functional.



#### Multicast Configuration
Addresses in the range 224.0.0.0 through 239.255.255.255 will switch will multicast the data. The multicast.xml file has `<retransmit>` set to:
```
  <retransmit>
    <address>234.234.234.234</address>
    <port>12345</port>
    <fields>ObjectIntersectionX,ObjectIntersectionY,ObjectIntersectionName</fields>
    <mode>binary</mode>
  </retransmit>
```
Build and run "CPPMulticastListener" or "CSMulticastListner" to listen for data over `234.234.234.234:12345`




### Data format
The adapter currently sends data in binary form.  The first 8 bytes will be a Windows Filetime.   The rest of the bytes will depend on the fields selected.   Most of the data fields are 32-bit floats.  Text strings are 64-bytes long.   

The `fields` example 
```
    <fields>ObjectIntersectionX,ObjectIntersectionY,ObjectIntersectionName</fields>
```
results in
* first 8 bytes as the UTC time. 
* The next 4 bytes are the ObjectIntersectionX
* The next 4 bytes are the ObjectIntersectionY
* the next 64 bytes are the ObjectIntersctionName

### Linux
The CPPListener.cpp and CPPMulticastListener.cpp files been compiled and tested on Ubuntu Linux 16.04.1 with g++ version 5.4.0.  They can be compiled by typing `g++ CPPListener.cpp` or `g++ CPPMulticastListener.cpp`
