using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using static teleBot.WeatherStructures;

namespace teleBot
{
    internal class Bot : IDisposable
    {
        private readonly TelegramBotClient _bot;
        private readonly Weather Weather;
        private readonly ILogger logger;
        private readonly tokens Tokens;
        public Bot()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();

            Tokens = config.GetSection("Settings").Get<tokens>();
            logger = loggerFactory.CreateLogger<Program>();
            Weather = new Weather(Tokens, logger);

            _bot = new TelegramBotClient(Tokens.BotToken) { Timeout = TimeSpan.FromSeconds(30) };
            logger.LogInformation($"Привет, я {_bot.GetMeAsync().Result.FirstName} и я помогу тебе узнать погоду.");
            _bot.OnMessage += Bot_OnMessage;
            _bot.StartReceiving();
        }
        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message.Type == MessageType.Text)
                {
                    if (message.Text == "/start")
                    {
                        await _bot.SendTextMessageAsync(message.Chat.Id, "Напиши мне город и я найду погоду.");
                        return;
                    }
                        
                    logger.LogInformation(message.Text);
                    if (await IsOnTheCityList(message.Text))
                    {
                        await Weather.GetWeatherFromSite(message.Text);
                        await ResponceToUser(message.Chat.Id);
                    }
                    else
                    {
                        await Weather.GetWeatherFromSite(message.Text);
                        if (Weather.WeatherResp.message == null)
                        {
                            await AddCityToList();
                            await ResponceToUser(message.Chat.Id);
                        }
                        else
                            await _bot.SendTextMessageAsync(message.Chat.Id, "Город не найден. Ошибка в орфографии.");
                        // добавить черный список и сверятся с ним
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex," ошибка бота");
            }
        }
        private async Task<bool> IsOnTheCityList(string city)
        {
            using var reader = new StreamReader(Tokens.CitiesListPath);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var cities = serializer.Deserialize<List<JsonCity>>(jsonReader);

            return cities.Exists(item => item.name == city);
        }
        private async Task AddCityToList()
        {
            using var reader = new StreamReader(Tokens.CitiesListPath);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var cities = serializer.Deserialize<List<JsonCity>>(jsonReader);
            reader.Close();

            var City = JsonConvert.DeserializeObject<JsonCity>(Weather.JsonResp);
            cities.Add(City);
            logger.LogInformation($"{ City.name} был добавлен в список городов.");

            using var writer = new StreamWriter(Tokens.CitiesListPath);
            using var jsonWriter = new JsonTextWriter(writer);
            serializer.Serialize(jsonWriter, cities);
            writer.Close();
            await Weather.SetNull();
        }
        private async Task ResponceToUser(long chatID)
        {
            var wind_degree = GetWindDegree();
            var WeatherResp = Weather.WeatherResp;
            var answer = $"Погода в городе {WeatherResp.name}\n" +
                $"{WeatherResp.weather[0].description}\n" +
                $"Ощущается как {Math.Round(WeatherResp.main.feels_like) - 273}°C\n" +
                $"Ветер {wind_degree}, скорость ветра {WeatherResp.wind.speed} м/c" +
                $"Средняя температура {Math.Round(WeatherResp.main.temp - 273)}°C \n " +
                $"Максимальная {Math.Round(WeatherResp.main.temp_max) - 273}°C, минимальная {Math.Round(WeatherResp.main.temp_min) - 273}°C\n" +
                $"Давление {WeatherResp.main.pressure}мм, влажность {WeatherResp.main.humidity}%\n";
            logger.LogInformation(answer);
            await _bot.SendTextMessageAsync(chatID, answer);
        }
        private string GetWindDegree()
        {
            switch (Weather.WeatherResp.wind.deg)
            {
                case int n when (67 < n && n <= 112):
                    return "восточноый";
                case int n when (112 < n && n <= 157):
                    return "юго-восточный";
                case int n when (157 < n && n <= 202):
                    return "южный";
                case int n when (202 < n && n <= 247):
                    return "юго-западный";
                case int n when (247 < n && n <= 292):
                    return "западный";
                case int n when (292 < n && n <= 337):
                    return "северо-западный";
                case int n when (337 < n && n <= 360 || 0 <= n && n <= 22):
                    return "северный";
                case int n when (22 < n && n <= 67):
                    return "северо-восточный";
            }
            return "err";
        }
        public void Dispose()
        {
            _bot.StopReceiving();
            logger.LogInformation("Бот перестал работать.");
        }
    }
}
