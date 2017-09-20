using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using System.Globalization;
    using System.Threading;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    public class Client
    {
        private String responseData;
        private int port = 13000;
        private TcpClient client; 
        private static Client instance;
        private String server = "localhost";
        private NetworkStream stream;
        Sensor sensor = Sensor.Insstance;
        public Vector3 CurrentPosition { get; set; }
        public Vector3 CurrentOrientation { get; set; }

        private Client()
        {
            Connect(server, port);
            Thread receiver = new Thread(Detecting);
            receiver.Start();
            receiver.IsBackground = true;
            Console.WriteLine("Connected to Robotics API!");
        }

        private void Detecting()
        {

            //responseData = String.Empty;
            //StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            //Sensor sensor = Sensor.Insstance;
            //try
            //{
            //    while (!reader.EndOfStream)
            //    {
            //        responseData = reader.ReadLine();

            //        JObject obj = JObject.Parse(responseData);
            //        float x = float.Parse(obj.GetValue("x").ToString(), CultureInfo.InvariantCulture.NumberFormat);
            //        float y = float.Parse(obj.GetValue("y").ToString(), CultureInfo.InvariantCulture.NumberFormat);
            //        float z = float.Parse(obj.GetValue("z").ToString(), CultureInfo.InvariantCulture.NumberFormat);
            //        float a = float.Parse(obj.GetValue("a").ToString(), CultureInfo.InvariantCulture.NumberFormat);
            //        float b = float.Parse(obj.GetValue("b").ToString(), CultureInfo.InvariantCulture.NumberFormat);
            //        float c = float.Parse(obj.GetValue("c").ToString(), CultureInfo.InvariantCulture.NumberFormat);

            //        Console.WriteLine("Received data: \n{0} {1} {2} {3} {4} {5}", x, y, z, a, b, c);
            //        CurrentPosition = new Vector3(x, y, z);
            //        CurrentOrientation = new Vector3(a, b, c);

            //    }
            //}
            //catch (IOException e)
            //{
            //    Console.WriteLine(e);
            //}
            //finally
            //{
            //    reader.Close();
            //}
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
            CultureInfo ci = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";

            string jsonData = "{ \"x\": \"" + x.ToString(ci) + "\", \"y\": \"" + y.ToString(ci) + "\", \"z\": \"" + z.ToString(ci) + "\", \"a\": \"" + a.ToString(ci) + "\", \"b\": \"" + b.ToString(ci) + "\", \"c\": \"" + c.ToString(ci) + "\" }";
            try
            {
                Send(jsonData);
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
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.WriteLine(message);

            Console.WriteLine("Sent: {0}", message);
            writer.Flush();
        }

        public void Receive()
        {
            responseData = String.Empty;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            try
            {
                while (!reader.EndOfStream)
                {
                    responseData = reader.ReadLine();

                    JObject obj = JObject.Parse(responseData);
                    float x = float.Parse(obj.GetValue("x").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(obj.GetValue("y").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float z = float.Parse(obj.GetValue("z").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float a = float.Parse(obj.GetValue("a").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float b = float.Parse(obj.GetValue("b").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float c = float.Parse(obj.GetValue("c").ToString(), CultureInfo.InvariantCulture.NumberFormat);

                    Console.WriteLine("Received data: \n{0} {1} {2} {3} {4} {5}", x, y, z, a, b, c);
                    CurrentPosition = new Vector3(x, y, z);
                    sensor.APIPosition = new Vector3(x, y, z);
                    CurrentOrientation = new Vector3(a, b, c);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                reader.Close();
            }
           
            
            //return new [] {x, y, z, a, b, c};
        }

        public void Connect(String server, int port)
        {
            client = new TcpClient(server, port);
            stream = client.GetStream();
        }

        public void Close()
        {
            stream.Close();
            client.Close();
        }
    }
}
