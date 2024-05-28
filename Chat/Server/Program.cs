using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static List<string> messageHistory = new List<string>();

        public static void StartListening(int port)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Ожидание соединения клиента...");
                    Socket handler = listener.Accept();

                    Console.WriteLine("Получение данных...");
                    byte[] buf = new byte[1024];
                    int bytesRec = handler.Receive(buf);
                    string data = Encoding.UTF8.GetString(buf, 0, bytesRec);

                    messageHistory.Add(data);
                    Console.WriteLine("Полученное сообщение: {0}", data);

                    string allMessages = string.Join("\n", messageHistory);
                    byte[] msg = Encoding.UTF8.GetBytes(allMessages);

                    handler.Send(msg);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet run <port>");
                return;
            }

            int port = int.Parse(args[0]);
            Console.WriteLine("Запуск сервера на порту {0}...", port);
            StartListening(port);
        }
    }
}
