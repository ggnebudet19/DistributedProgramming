using StackExchange.Redis;

namespace Valuator
{
    public class TextEvaluator
    {
        public static int CalculateSimilarity(string text, IDatabase db, string currentKey)
        {
            if (IsDuplicate(text, db, currentKey))
                return 1;
            else
                return 0;
        }

        private static bool IsDuplicate(string text, IDatabase db, string currentKey)
        {
            const string textKeyPrefix = "TEXT-";

            var textKeys = db.Multiplexer.GetServer("127.0.0.1:6379").Keys(pattern: textKeyPrefix + "*");

            foreach (var key in textKeys)
            {
                if (key != currentKey)
                {
                    string storedText = db.StringGet(key).ToString() ?? string.Empty;

                    if (string.Equals(storedText, text, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
