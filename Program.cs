using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using static teleBot.WeatherSiteResponce;


namespace teleBot
{
    internal class Program
    {
        private static TelegramBotClient _bot;
        private static WeatherResponse _weatherResp;
        private static string _jsonResp;
        
        static void Main(string[] args)
        {
            _bot = new TelegramBotClient(tokens.botToken) { Timeout = TimeSpan.FromSeconds(30) };
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
                if (await isOnTheCityList(message.Text))
                {
                    //await GetWeatherFromSite(message.Text); 
                    _weatherResp = JsonConvert.DeserializeObject<WeatherResponse>(tokens.testJson);
                    await responceToUser(message.Chat.Id);
                }
                else
                {
                    _weatherResp = JsonConvert.DeserializeObject<WeatherResponse>(tokens.testJson);
                    _jsonResp = tokens.testJson;
                    //await GetWeatherFromSite(message.Text);
                    if (_weatherResp.message == null)
                    {
                        await AddCityToList();
                        await responceToUser(message.Chat.Id);
                    }
                    else
                        await _bot.SendTextMessageAsync(message.Chat.Id, "Город не найден. Ошибка в орфографии.");
                    // добавить черный список и сверятся с ним
                }
                    
            }
        }
        public static async Task GetWeatherFromSite(string cityName)
        {
            try
            {
                var url = $"{tokens.weatherSite}?q={cityName}&unit=metric&appid={tokens.weatherSiteAppID}&lang=ru";
                var httpWebResponse = WebRequest.Create(url).GetResponse();
                using var reader = new StreamReader(httpWebResponse.GetResponseStream());
                _jsonResp = reader.ReadToEnd();
                Console.WriteLine(_jsonResp);
                _weatherResp = JsonConvert.DeserializeObject<WeatherResponse>(_jsonResp);
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        public static async Task<bool> isOnTheCityList(string city)
        {
            using var reader = new StreamReader(tokens.citiesList_path);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var cities = serializer.Deserialize<List<JsonCity>>(jsonReader);

            if (cities.Exists(item => item.name == city))
                return true;
            return false;
        }
        public static async Task AddCityToList()
        {
            using var reader = new StreamReader(tokens.citiesList_path);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var cities = serializer.Deserialize<List<JsonCity>>(jsonReader);
            reader.Close();

            cities.Add(JsonConvert.DeserializeObject<JsonCity>(_jsonResp));

            using var writer = new StreamWriter(tokens.citiesList_path);
            using var jsonWriter = new JsonTextWriter(writer);
            serializer.Serialize(jsonWriter, cities);
            writer.Close();
        }
        public static async Task responceToUser(long chatID)
        {
            var answer = $"Температура в {_weatherResp.name}: {Math.Round(_weatherResp.main.temp - 273)} °C"; // сделать расширенный ответ
            Console.WriteLine(answer);
            await _bot.SendTextMessageAsync(chatID, answer);
        }
    }

}


