using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace SampleEventSource
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup UDP for Event Adapter
            var client = new UdpClient();
            string address = "127.0.0.1";
            int port = 5992;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(address), port); 
            client.Connect(ep);

            // setup Event data
            string message = "None";  // default message should be 'None'
            int group = 1;
            int flag = 0;  

            // loop until ESC
            Console.WriteLine("Sending data to Event adapter. Press 'ESC' key to quit.");
            bool done = false;
            while (!done)
            {
                if (Console.KeyAvailable)
                {
                   // Console.ReadKey(true).Key == ConsoleKey.Escape));

                    switch(Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.Escape:
                            done = true;
                            break;
                        case ConsoleKey.A:
                            message = "Phase A";
                            group = 1;
                            break;
                        case ConsoleKey.B:
                            message = "Phase B";
                            group = 2;
                            break;
                        case ConsoleKey.C:
                            message = "Phase C";
                            group = 2;
                            break;
                        case ConsoleKey.D:
                            message = "Phase D";
                            group = 2;
                            break;
                    }
                }

                // setup the message with UTC in filetime
                var timestamp = DateTime.UtcNow.ToFileTimeUtc(); 
                string toSend = timestamp.ToString() + "," + message + "," + group + "," + flag;

                // send the message out to the network adapter
                byte[] toBytes = Encoding.ASCII.GetBytes(toSend);
                client.Send(toBytes, toBytes.Length);

                // The standard Event's setup is to expect messages at 1Hz
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}