using System;
using System.Net.Sockets;
using System.Text;

namespace Chat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: dotnet run <host> <port> <message>");
                return;
            }

            string host = args[0];
            int port = int.Parse(args[1]);
            string message = args[2];

            try
            {
                TcpClient client = new TcpClient(host, port);
                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine(response);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
