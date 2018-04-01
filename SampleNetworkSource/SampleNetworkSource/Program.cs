using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;


namespace SampleNetworkSource
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup UDP client connection.
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4400);
            client.Connect(ep);

            float time = 0.0f; 
            int counter = 0;
            // adapter started, set 
            Console.WriteLine("Sending data to network adapter. Press 'ESC' key to quit.");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                //  get UTC time as Windows Filetime.
                //  http://www.silisoftware.com/tools/date.php
                //  var timestamp = DateTime.UtcNow.AddSeconds(-15).ToFileTimeUtc();  // Use this to demonstrate Skew in RM
                var timestamp = DateTime.UtcNow.ToFileTimeUtc(); 
                string msg = timestamp.ToString();

                // Add 'SomeTextField' to the network adapter message
                msg += ",Some Text";

                // Add 'AnIntegerField' to the network adapter message
                int someInt = counter * 2;
                msg += "," + someInt;

                // Add 'SomeFloatField' to the network adapter message
                float cosAngle = (float)Math.Cos(time);
                msg += "," + cosAngle;

                // Add 'SomeDoubleField' to the network adapter message
                double sinAngle = Math.Sin(time);
                msg += "," + sinAngle;

                // Add 'Some64BitIntField' to the network adapter message
                var max64 = Int64.MaxValue -counter;
                msg += "," + max64;
                  
                // send the message out to the network adapter
                byte[] toBytes = Encoding.ASCII.GetBytes(msg);
                client.Send(toBytes, toBytes.Length);

                // just a pause to hit a frequency ( 50Hz ->  20 ms,   10Hz -> 100ms )
                //System.Threading.Thread.Sleep(100);
                System.Threading.Thread.Sleep(20);
                time += 0.1f;
                counter += 1;
            }
            Console.WriteLine("Quitting.");
        }
    }
}
