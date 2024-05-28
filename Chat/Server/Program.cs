using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat.Server
{
    class Program
    {
        private static List<string> messageHistory = new List<string>();

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet run <port>");
                return;
            }

            int port = int.Parse(args[0]);
            TcpListener listener = new TcpListener(IPAddress.Any, port);

            listener.Start();
            Console.WriteLine($"Server started on port {port}");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Message received: {message}");
                messageHistory.Add(message);

                string history = string.Join("\n", messageHistory);
                byte[] response = Encoding.UTF8.GetBytes(history);
                stream.Write(response, 0, response.Length);

                client.Close();
            }
        }
    }
}
