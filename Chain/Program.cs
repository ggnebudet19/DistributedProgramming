using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chain
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: dotnet run <listening-port> <next-host> <next-port> [true]");
                return;
            }

            int listeningPort = int.Parse(args[0]);
            string nextHost = args[1];
            int nextPort = int.Parse(args[2]);
            bool isInitiator = args.Length == 4 && args[3] == "true";

            Console.Write("Enter the value of X: ");
            int localX = int.Parse(Console.ReadLine());

            if (isInitiator)
            {
                await InitiateAlgorithm(localX, listeningPort, nextHost, nextPort);
            }
            else
            {
                await ParticipateInAlgorithm(localX, listeningPort, nextHost, nextPort);
            }
        }

        static async Task InitiateAlgorithm(int localX, int listeningPort, string nextHost, int nextPort)
        {
            await SendValue(nextHost, nextPort, localX);

            int receivedX = await ReceiveValue(listeningPort);

            int maxX = Math.Max(localX, receivedX);

            await SendValue(nextHost, nextPort, maxX);

            int finalMaxX = await ReceiveValue(listeningPort);

            Console.WriteLine(finalMaxX);
        }

        static async Task ParticipateInAlgorithm(int localX, int listeningPort, string nextHost, int nextPort)
        {
            int receivedX = await ReceiveValue(listeningPort);

            int maxX = Math.Max(localX, receivedX);

            await SendValue(nextHost, nextPort, maxX);

            int finalMaxX = await ReceiveValue(listeningPort);

            Console.WriteLine(finalMaxX);
        }

        static async Task<int> ReceiveValue(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            using (TcpClient client = await listener.AcceptTcpClientAsync())
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[4];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                int receivedValue = BitConverter.ToInt32(buffer, 0);
                return receivedValue;
            }
        }

        static async Task SendValue(string host, int port, int value)
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(host, port);
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = BitConverter.GetBytes(value);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
