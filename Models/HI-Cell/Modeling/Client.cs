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
    using ThreadPriority = System.Threading.ThreadPriority;

    public class Client
    {
        private bool Connected = false;
        private String responseData;
        private int port = 13000;
        private TcpClient client; 
        private static Client instance;
        private String server = "localhost";
        private NetworkStream stream;
        public Thread receiver;
        public Vector3 CurrentPosition { get; set; }
        public Vector3 CurrentOrientation { get; set; }

        private Client()
        {
            if (!Connected)
                Connect(server, port);
        }

        public static Client getInstance
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
            Robot robot = Robot.getInstance;
            int offset = 10;
            Console.WriteLine("Started Receiving...");
            responseData = String.Empty;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            try
            {
                while (true)
                {
                    if (!client.Connected)
                    {
                        throw new IOException();
                    }
                    responseData = reader.ReadLine();
                    
                    JObject obj = JObject.Parse(responseData);
                    float x = float.Parse(obj.GetValue("x").ToString(), CultureInfo.InvariantCulture.NumberFormat) * offset;
                    float y = float.Parse(obj.GetValue("y").ToString(), CultureInfo.InvariantCulture.NumberFormat) * offset;
                    float z = float.Parse(obj.GetValue("z").ToString(), CultureInfo.InvariantCulture.NumberFormat) * offset;
                    float a = float.Parse(obj.GetValue("a").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float b = float.Parse(obj.GetValue("b").ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float c = float.Parse(obj.GetValue("c").ToString(), CultureInfo.InvariantCulture.NumberFormat);

                    Console.WriteLine("Received data: \n{0} {1} {2} {3} {4} {5}", x, y, z, a, b, c);
                    if (Math.Abs(x - robot.GetXCoord()) < 0.00001 && Math.Abs(y - robot.GetYCoord()) < 0.00001 &&
                        Math.Abs(z - robot.GetZCoord()) < 0.00001)
                    {
                        robot.Stop();
                    }
                    //if (robot.HasStopped && !robot.SamePositionAsTarg)
                    //    Sensor.getInstance.ObstInEnvironment = true;
                    CurrentPosition = new Vector3(x, y, z);
                    Robot.getInstance.Position = CurrentPosition;
                    CurrentOrientation = new Vector3(a, b, c);

                    Console.WriteLine("Current Position: "+CurrentPosition);
                    Console.WriteLine("Reached end of while loop");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                reader.Close();
                stream.Close();
                Environment.Exit(0);
            }
        }

        public void Connect(String server, int port)
        {
            client = new TcpClient(server, port);
            stream = client.GetStream();
            Connected = true;
            Console.WriteLine("Connected to Robotics API!");

            receiver = new Thread(Receive);
            receiver.Priority = ThreadPriority.AboveNormal;
            receiver.IsBackground = true;
            receiver.Start();
        }

        public void Close()
        {
            stream.Close();
            client.Close();
            Connected = false;
        }
    }
}
