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
        public static async Task GetWeatherFromSite(string cityName)
        {
            try
            {
                var url = $"{tokens.weatherSite_weather}?q={cityName}&unit=metric&appid={tokens.weatherSiteAppID}&lang=ru";
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
