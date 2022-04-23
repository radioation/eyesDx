using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SampleBinarySource
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup UDP client connection.
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23223);
            client.Connect(ep);

            float time = 0.0f;
            int counter = 0;
            // adapter started, set 
            Console.WriteLine("Sending data to network adapter. Press 'ESC' key to quit.");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                var outBuffer = new MemoryStream();
                var binaryWriter = new BinaryWriter(outBuffer);
                
                //  get UTC time as Windows Filetime.
                //  http://www.silisoftware.com/tools/date.php
                var timestamp = DateTime.UtcNow.ToFileTimeUtc();
                binaryWriter.Write(timestamp);

                // Add 'SomeTextField' to the network adapter message
                string message = "Some C# Text Field";
                int length = 64;  
                // Defined String64 in confiruration XML. So we need to pad it
                string paddedMessage = message.PadRight(length).Substring(0, length);
                Console.WriteLine("'{0}'", paddedMessage);
                binaryWriter.Write(paddedMessage.ToCharArray());


                // Add 'AnIntegerField' to the network adapter message
                int someInt = counter;
                binaryWriter.Write(someInt);

                // Add 'SomeFloatField' to the network adapter message
                float cosAngle = (float)Math.Cos(time);
                binaryWriter.Write(cosAngle);

                // Add 'SomeDoubleField' to the network adapter message
                double sinAngle = Math.Sin(time);
                binaryWriter.Write(sinAngle);

                // Add 'Some64BitIntField' to the network adapter message
                var max64 = Int64.MaxValue - counter;
                binaryWriter.Write(max64);

                binaryWriter.Close();
                byte[] bytes = outBuffer.ToArray();

                // send the message out to the network adapter 
                client.Send(bytes, bytes.Length);

                // just a pause to hit a frequency ( 50Hz ->  20 ms,   10Hz -> 100ms )
                //System.Threading.Thread.Sleep(100);
                System.Threading.Thread.Sleep(20);
                time += 0.1f;
                counter += 1;
                if( counter > 2000 )
                {
                    counter = 0;
                }
            }
            Console.WriteLine("Quitting.");
        }
    }
}
