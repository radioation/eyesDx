using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using System.Net.Sockets;

namespace SERetransmitExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Creates a UdpClient for reading incoming data.
            UdpClient receivingUdpClient = new UdpClient();  // we're listening to port 51500
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 51500);  // any source

            Console.WriteLine("listening for retransmitted eye tracking data. Press 'ESC' key to quit.");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
 
                try
                {

                    // Wait for bytes to come in
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                    // convert to values.
                    var fileTime = BitConverter.ToUInt64(receiveBytes, 0);
                    var x = BitConverter.ToSingle(receiveBytes, 8);
                    var y = BitConverter.ToSingle(receiveBytes, 12);
                    string name = Encoding.ASCII.GetString(receiveBytes, 16, 64);

                    Console.WriteLine("Filetime: {0} x:{1} y:{2} objectName: {3}", fileTime, x, y, name);
                  
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            Console.WriteLine("Quitting.");

        }
    }
}
