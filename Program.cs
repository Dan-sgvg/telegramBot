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
using System.Drawing;

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
                    await GetWeatherFromSite(message.Text); 
                    await responceToUser(message.Chat.Id);
                }
                else
                {
                    await GetWeatherFromSite(message.Text);
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
        private static async Task GetWeatherFromSite(string cityName)
        {
            try
            {
                var url = $"{tokens.weatherSite_weather}?q={cityName}&unit=metric&appid={tokens.weatherSiteAppID}&lang=ru";
                var httpWebResponse = WebRequest.Create(url).GetResponse();
                using var reader = new StreamReader(httpWebResponse.GetResponseStream());
                _jsonResp = reader.ReadToEnd();
                _weatherResp = JsonConvert.DeserializeObject<WeatherResponse>(_jsonResp);
                _weatherResp.message = null;
            }
            catch (WebException e)
            {
                _weatherResp.message = "err";
                Console.WriteLine(e);
                return;
            }
        }
        private static async Task<bool> isOnTheCityList(string city)
        {
            using var reader = new StreamReader(tokens.citiesList_path);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var cities = serializer.Deserialize<List<JsonCity>>(jsonReader);

            if (cities.Exists(item => item.name == city))
                return true;
            return false;
        }
        private static async Task AddCityToList()
        {
            using var reader = new StreamReader(tokens.citiesList_path);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var cities = serializer.Deserialize<List<JsonCity>>(jsonReader);
            reader.Close();

            var City = JsonConvert.DeserializeObject<JsonCity>(_jsonResp);
            cities.Add(City);
            Console.WriteLine($"{ City.name} был добавлен в список городов.");

            using var writer = new StreamWriter(tokens.citiesList_path);
            using var jsonWriter = new JsonTextWriter(writer);
            serializer.Serialize(jsonWriter, cities);
            writer.Close();
            _jsonResp = null;
        }
        private static async Task responceToUser(long chatID)
        {
            var wind_degree = getWindDegree();
            var answer = $"Погода в городе {_weatherResp.name}\n" +
                $"Средняя температура {Math.Round(_weatherResp.main.temp - 273)}°C \n " +
                $"Максимальная {Math.Round(_weatherResp.main.temp_max) - 273}°C, минимальная {Math.Round(_weatherResp.main.temp_min) - 273}°C\n" +
                $"{_weatherResp.weather[0].description}\n" +
                $"Ощущается как {Math.Round(_weatherResp.main.feels_like) - 273}°C\n" +
                $"Давление {_weatherResp.main.pressure}, влажность {_weatherResp.main.humidity}%\n" +
                $"Ветер {wind_degree}, скорость ветра {_weatherResp.wind.speed} м/c";
            Console.WriteLine(answer);
            await _bot.SendTextMessageAsync(chatID, answer); 
        }
        private static string getWindDegree()
        {
            switch (_weatherResp.wind.deg)
            {
                case int n when (67 < n && n <= 112):
                    return "восточноый"; break;
                case int n when (112 < n && n <= 157):
                    return "юго-восточный"; break;
                case int n when (157 < n && n <= 202):
                    return "южный"; break;
                case int n when (202 < n && n <= 247):
                    return "юго-западный"; break;
                case int n when (247 < n && n <= 292):
                    return "западный"; break;
                case int n when (292 < n && n <= 337):
                    return "северо-западный"; break;
                case int n when (337 < n && n <= 360 || 0 <= n && n <= 22):
                    return "северный"; break;
                case int n when (22 < n && n <= 67):
                    return "северо-восточный"; break;
            }
            return "err";
        }
    }

}


