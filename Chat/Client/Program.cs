using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        public static void StartClient(string host, int port, string message)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                Socket sender = new Socket(
                                ipAddress.AddressFamily,
                                SocketType.Stream,
                                ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine("Удалённый адрес подключения сокета: {0}",
                                      sender?.RemoteEndPoint?.ToString() ?? "Адрес недоступен");

                    byte[] msg = Encoding.UTF8.GetBytes(message);
                    if (sender == null)
                    {
                        throw new ArgumentNullException(nameof(sender));
                    }
                    sender.Send(msg);

                    byte[] buf = new byte[1024];
                    int bytesRec = sender.Receive(buf);
                    Console.WriteLine("История сообщений:\n{0}", Encoding.UTF8.GetString(buf, 0, bytesRec));

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

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

            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine("The message cannot be empty!");
                return;
            }

            StartClient(host, port, message);
        }
    }
}
