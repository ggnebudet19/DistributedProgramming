using NATS.Client;
using System;
using System.Text;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EventsLogger started");
            ConnectionFactory cf = new ConnectionFactory();

            using IConnection c = cf.CreateConnection();
            {
                var similaritySubscription = c.SubscribeAsync("valuator.events.similarity", (sender, args) =>
                {
                    string message = Encoding.UTF8.GetString(args.Message.Data);
                    Console.WriteLine($"SimilarityCalculated event received: {message}");
                });
                similaritySubscription.Start();

                var rankSubscription = c.SubscribeAsync("valuator.events.rank", (sender, args) =>
                {
                    string message = Encoding.UTF8.GetString(args.Message.Data);
                    Console.WriteLine($"RankCalculated event received: {message}");
                });
                rankSubscription.Start();

                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();

                similaritySubscription.Unsubscribe();
                rankSubscription.Unsubscribe();

                c.Drain();
                c.Close();
            }
        }
    }
}
