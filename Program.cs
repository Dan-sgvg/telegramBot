using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace teleBot
{
    internal class Program
    {
        private static TelegramBotClient _bot;
        private static WeatherResponse WR;

        //перенести в отдельный файл
        private static readonly string token = ""; 
        private static readonly string weatherSite = "https://api.openweathermap.org/data/2.5/weather";
        private static readonly string weatherSiteAppID = ""; 
        
        static void _Main(string[] args)
        {
            _bot = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(30) };
            Console.WriteLine($"Привет, я {_bot.GetMeAsync().Result.FirstName} и я помогу тебе узнать погоду.");
            _bot.OnMessage += Bot_OnMessage;
            _bot.StartReceiving();
            Console.ReadLine();
            _bot.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message.Type == MessageType.Text)
            {
                Console.WriteLine(message.Text);
                if (1 == 1) //если существует в citiesListRU
                {
                    await GetWeather(message.Text); 
                    var answer = $"Температура в {WR.name}: {Math.Round(WR.main.temp)} °C"; // сделать расширенный ответ
                    Console.WriteLine(answer);
                    await _bot.SendTextMessageAsync(message.Chat.Id, answer);
                }
                else
                    await _bot.SendTextMessageAsync(message.Chat.Id, "Город не найден. Ошибка в орфографии.");
            }
        }
        public static async Task GetWeather(string cityName)
        {
            try
            {
                var url = $"{weatherSite}?q={cityName}&unit=metric&appid={weatherSiteAppID}&lang=ru";
                var httpWebResponse = WebRequest.Create(url).GetResponse();
                using var reader = new StreamReader(httpWebResponse.GetResponseStream());
                var response = reader.ReadToEnd();
                WR = JsonConvert.DeserializeObject<WeatherResponse>(response);
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                return;
            }
        }
        //перенести в отдельный файл
        public class WeatherResponse
        {
            public Weather[] weather { get; set; } // id, параметры погоды, описание, иконка
            public Main main { get; set; } // темп, ощущается, max, min, давление, влажность
            public Wind wind { get; set; } // скорость, "наклон" ветра
            public Clouds clouds { get; set; } //% облачности
            public string name { get; set; } //город
        }
        public class Main
        {
            public float temp { get; set; }
            public float feels_like { get; set; }
            public float temp_min { get; set; }
            public float temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
        }
        public class Wind
        {
            public float speed { get; set; }
            public int deg { get; set; }
        }
        public class Clouds
        {
            public int all { get; set; }
        }
        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

    }

}


