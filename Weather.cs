using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static teleBot.WeatherStructures;
using Microsoft.Extensions.Logging;

namespace teleBot
{
    internal class Weather
    {
        public WeatherResponseJsonClass WeatherResp { get; private set; }
        public string JsonResp { get; private set; }
        public tokens Tokens { get; private set; }
        public ILogger logger { get; private set; }
        private readonly HttpClient client = new HttpClient();

        public Weather(tokens Tokens, ILogger Logger)
        {
            this.Tokens = Tokens;
            this.logger = Logger;
        }
        public async Task GetWeatherFromSite(string cityName)
        {
            try
            {
                var url = $"{Tokens.WeatherSiteUrl}?q={cityName}&unit=metric&appid={Tokens.WeatherSiteAppID}&lang=ru";
                var response = await client.GetAsync(url);
                //client.GetFromJsonAsync<WeatherResponseJsonClass>(url);
                response.EnsureSuccessStatusCode();
                JsonResp = await response.Content.ReadAsStringAsync();
                WeatherResp = JsonConvert.DeserializeObject<WeatherResponseJsonClass>(JsonResp);
                WeatherResp.message = null;
            }
            catch (HttpRequestException e)
            {
                logger.LogError(e, "err");
            }
        }
        public async Task SetNull()
        {
            JsonResp = null;
        }
    }
}
