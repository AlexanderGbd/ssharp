using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace SafetySharp.CaseStudies.HI_Cell.Modeling
{
    using System.Globalization;
    using System.Threading;
    using Newtonsoft.Json.Linq;
    using UnityEngine;
    using ThreadPriority = System.Threading.ThreadPriority;

    public class Client
    {
        public static bool Running;

        private Model _model;
        public bool Connected = false;
        private String responseData;
        private int port = 13000;
        private TcpClient client; 
        private static Client instance;
        private String server = "localhost";
        private NetworkStream stream;
        public Thread receiver;
        public Vector3 CurrentPosition { get; set; }
        public Vector3 CurrentOrientation { get; set; }
        public bool SamePositionAsTarget;
        public bool ObstacleDetectedDuringMovement;
        public bool RobotIsMoving;

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
            CultureInfo ci = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
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
            int offset = 10;
            Console.WriteLine("Started Receiving...");
            responseData = String.Empty;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            try
            {
                while (Running)
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

                    RobotIsMoving = true;

                    Console.WriteLine("Received data: \n{0} {1} {2} {3} {4} {5}", x, y, z, a, b, c);

                    //Position hasn't changed since last value
                    if (Math.Abs(x - CurrentPosition.x) < 0.0001 && Math.Abs(y - CurrentPosition.y) < 0.0001 &&
                        Math.Abs(z - CurrentPosition.z) < 0.0001)
                    {
                        RobotIsMoving = false;
                        //Did the robot stop because he reached its target or because of an obstacle?
                        if (Math.Abs(CurrentPosition.x - Model.XTarget) < 0.0001 && Math.Abs(CurrentPosition.y - Model.YTarget) < 0.0001 && Math.Abs(CurrentPosition.z - Model.ZTarget) < 0.0001)
                        {
                            SamePositionAsTarget = true;
                        }
                        else
                        {
                            SamePositionAsTarget = false;
                            ObstacleDetectedDuringMovement = true;
                        }
                    }
                    //Precisely would be:
                    //if (Math.Abs(x - StaticObstacle.Position.x) < 0.0001 && Math.Abs(y - StaticObstacle.Position.y) < 0.0001
                    //    && Math.Abs(z - StaticObstacle.Position.z) < 0.0001)
                    if ((int) x == (int) StaticObstacle.Position.x && (int) y == (int) StaticObstacle.Position.y && (int)z == (int) StaticObstacle.Position.z)
                    {
                        ObstacleDetectedDuringMovement = true;
                    }
                    
                    CurrentPosition = new Vector3(x, y, z);
                    CurrentOrientation = new Vector3(a, b, c);

                    Console.WriteLine("Current Position: " + CurrentPosition);
                    Console.WriteLine("Reached end of while loop");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Disconnected");
                Console.WriteLine(e);
                Environment.Exit(0);
            }
            finally
            {
                reader.Close();
                Console.WriteLine("Disconnected");
            }
        }

        public void Connect(String server, int port)
        {
            client = new TcpClient(server, port);
            stream = client.GetStream();
            Connected = true;
            Running = true;
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

        public void Reconnect()
        {
            Connect(server, port);
        }
    }
}
