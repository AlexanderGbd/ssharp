using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using MahApps.Metro.Actions;
    using UnityEngine;

    public class Client
    {
        private String responseData;
        private int port = 13000;
        private TcpClient client; 
        private static Client instance;
        private String server = "localhost";
        private NetworkStream stream;

        private Client()
        {
            Connect(server, port);
            Console.WriteLine("Connected to Robotics API!");
        }

        public static Client Instance
        {
            get
            {
                if (instance == null)
                    instance = new Client();
                return instance;
            }
        }


        public void MoveDirectlyTo(double x, double y, double z, double a, double b, double c)
        {
            //string jsonString = string.Format("{0}{1}{2}{3}{4}{5}", x, y, z, a, b, c);
            CultureInfo ci = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";

            string jsonData = "{ \"x\": \"" + x.ToString(ci) + "\", \"y\": \"" + y.ToString(ci) + "\", \"z\": \"" + z.ToString(ci) + "\", \"a\": \"" + a.ToString(ci) + "\", \"b\": \"" + b.ToString(ci) + "\", \"c\": \"" + c.ToString(ci) + "\" }";
            try
            {
                Send(jsonData);
                //Receive();
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0}", e);
            }
  
        }

        public void MoveCollisionFreeTo(float x, float y, float z, float a, float b, float c)
        {
            //To be implemented
        }

        public void Send(String message)
        {
            // Translate the passed message into ASCII and store it as a Byte array.

            //Byte[] data = Encoding.ASCII.GetBytes(message);
            //stream.Write(data, 0, data.Length);

            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.WriteLine(message);

            Console.WriteLine("Sent: {0}", message);
            writer.Flush();
        }

        public void Receive()
        {
            // Buffer to store the response bytes.
            //Byte[] data = new Byte[256];

            // Read the first batch of the TcpServer response bytes.
            //Int32 bytes = stream.Read(data, 0, data.Length);
            //responseData = Encoding.ASCII.GetString(data, 0, bytes);

            // String to store the response ASCII representation.
            responseData = String.Empty;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            while (!reader.EndOfStream)
            {

            }
            responseData = reader.ReadLine();
            
            Console.WriteLine("Received: {0}", responseData);
        }

        public void Connect(String server, int port)
        {
            client = new TcpClient(server, port);
            stream = client.GetStream();
        }

        //public void Con(String server, String message)
        //{
        //    try
        //    {
        //        Int32 port = 13000;
        //        TcpClient client = new TcpClient(server, port);

        //        // Translate the passed message into ASCII and store it as a Byte array.
        //        Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

        //        // Get a client stream for reading and writing.
        //        NetworkStream stream = client.GetStream();

        //        // Send the message to the connected TcpServer. 
        //        stream.Write(data, 0, data.Length);

        //        Console.WriteLine("Sent: {0}", message);

        //        // Buffer to store the response bytes.
        //        data = new Byte[256];

        //        // String to store the response ASCII representation.
        //        String responseData = String.Empty;

        //        // Read the first batch of the TcpServer response bytes.
        //        Int32 bytes = stream.Read(data, 0, data.Length);
        //        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //        Console.WriteLine("Received: {0}", responseData);
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        Console.WriteLine("ArgumentNullException: {0}", e);
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("SocketException: {0}", e);
        //    }
        //}

        public void Close()
        {
            stream.Close();
            client.Close();
        }

        public Vector3 GetCurrentPosition()
        {
            float x, y, z, a, b, c;
            Regex regex = new Regex(",");
            string[] values = regex.Split(responseData);
            x = (float) Convert.ToDouble(values[0]);

            return new Vector3(0,0,0);
        }
    }
}
