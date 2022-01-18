using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static teleBot.WeatherStructures;


namespace teleBot
{
    internal class Weather
    {
        public static WeatherResponseJsonClass WeatherResp;
        public static string _jsonResp;
        private static tokens Tokens;
        public Weather(tokens tokens)
        {
            Tokens = tokens;
        }

        public static async Task GetWeatherFromSite(string cityName)
        {
            try
            {
                var url = $"{Tokens.WeatherSiteUrl}?q={cityName}&unit=metric&appid={Tokens.WeatherSiteAppID}&lang=ru";
                var httpWebResponse = WebRequest.Create(url).GetResponse();
                using var reader = new StreamReader(httpWebResponse.GetResponseStream());
                _jsonResp = reader.ReadToEnd();
                WeatherResp = JsonConvert.DeserializeObject<WeatherResponseJsonClass>(_jsonResp);
                WeatherResp.message = null;
            }
            catch (WebException e)
            {
                WeatherResp.message = "err";
                Console.WriteLine(e);
            }
        }
        
    }
}
