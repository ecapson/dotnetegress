using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace dotnetegress
{

    public class GetSocket
    {
        string server;
        int port;

        public GetSocket(string iserver, int iport)
        {
            server = iserver;
            port = iport;
        }
        private static Socket ConnectSocketIP(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            IPEndPoint ipe = IPEndPoint.Parse(server + ':' + port.ToString());
            Socket tempSocket =
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.ReceiveBufferSize = 1024;
            tempSocket.ReceiveTimeout = 1000;
            tempSocket.SendTimeout = 1000;
            tempSocket.LingerState = new LingerOption(false, 0);
            tempSocket.NoDelay = true;

            try
            {
                tempSocket.Connect(ipe);
                s = tempSocket;
            }
            catch
            {
                s = null;
            }
            return s;
        }

        public void SocketSendReceive()
        {
            string request = port.ToString();
            byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            byte[] bytesReceived = new byte[1024];

            try
            {
                Socket s = ConnectSocketIP(server, port);
                s.Send(bytesSent, bytesSent.Length, 0);
                int bytes = 0;
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                System.Console.WriteLine("Connection successful on port: " + port.ToString());

            }
            catch
            {
                System.Console.WriteLine("Connection failed on port: " + port.ToString());
            }

        }
    }

    public class Egress
    {
        public static void Main(string[] args)
        {
            string host = null;
            string portrange = null;
            int lowport = 0;
            int highport = 0;

            if (args.Length < 2)
            {

                System.Console.WriteLine("requires host IP and port range (ex: 127.0.0.1 1-65535)");
                System.Environment.Exit(0);
            }
            else
            {
                host = args[0];
                portrange = args[1];
                lowport = System.Convert.ToInt32(portrange.Split('-')[0]);
                highport = System.Convert.ToInt32(portrange.Split('-')[1]);
            }
            for (int port = lowport; port < highport + 1; port++)
            {
                GetSocket s = new GetSocket(host, port);
                Thread thread = new Thread(new ThreadStart(s.SocketSendReceive));
                thread.Start();
                Thread.Sleep(10);
            }
        }
    }
}