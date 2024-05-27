using NATS.Client;
using StackExchange.Redis;
using System;
using System.Text;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RankCalculator started");
            ConnectionFactory cf = new ConnectionFactory();

            using IConnection c = cf.CreateConnection();
            {            
                var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
                {
                    string message = Encoding.UTF8.GetString(args.Message.Data);
                    Console.WriteLine("Consuming: {0} from subject {1}", message, args.Message.Subject);

                    ProcessMessage(message, c);
                });

                s.Start();

                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();

                s.Unsubscribe();

                c.Drain();
                c.Close();
            }
        }

        private static void ProcessMessage(string message, IConnection natsConnection)
        {
            var parts = message.Split(':');
            if (parts.Length != 2) return;

            string id = parts[0];
            string text = parts[1];

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            IDatabase db = redis.GetDatabase();

            int totalCharacters = text.Length;
            int nonAlphabeticCharacters = text.Count(c => !char.IsLetter(c));

            double rank = (double)nonAlphabeticCharacters / totalCharacters;

            string rankKey = "RANK-" + id;
            db.StringSet(rankKey, rank);

            string rankMessage = $"{id}:{rank}";
            byte[] rankData = Encoding.UTF8.GetBytes(rankMessage);
            natsConnection.Publish("valuator.events.rank", rankData);
        }
    }
}
