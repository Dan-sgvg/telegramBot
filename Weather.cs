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
        public WeatherResponseJsonClass WeatherResp;
        public string JsonResp;
        private tokens Tokens;
        public Weather(tokens tokens)
        {
            Tokens = tokens;
        }

        public async Task GetWeatherFromSite(string cityName)
        {
            try
            {
                var url = $"{Tokens.WeatherSiteUrl}?q={cityName}&unit=metric&appid={Tokens.WeatherSiteAppID}&lang=ru";
                var httpWebResponse = WebRequest.Create(url).GetResponse();
                using var reader = new StreamReader(httpWebResponse.GetResponseStream());
                JsonResp = reader.ReadToEnd();
                WeatherResp = JsonConvert.DeserializeObject<WeatherResponseJsonClass>(JsonResp);
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
