namespace Valuator
{
    public class CountryHandler
    {
        public static string GetRegionByCountry(string country)
        {
            return country switch
            {
                "Russia" => "RUS",
                "France" => "EU",
                "Germany" => "EU",
                "USA" => "OTHER",
                "India" => "OTHER",
                _ => "OTHER",
            };
        }
    }
}
