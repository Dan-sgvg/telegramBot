using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static teleBot.WeatherSiteResponce;

namespace teleBot
{
    internal class Program
    {
        private static TelegramBotClient _bot;
        private static WeatherResponse WR;
        
        
        static void _Main(string[] args)
        {
            _bot = new TelegramBotClient(tokens.token) { Timeout = TimeSpan.FromSeconds(30) };
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
                var url = $"{tokens.weatherSite}?q={cityName}&unit=metric&appid={tokens.weatherSiteAppID}&lang=ru";
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

    }

}


