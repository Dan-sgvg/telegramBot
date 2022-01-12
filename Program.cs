using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;

namespace teleBot
{
    internal class Program
    {
        private static TelegramBotClient _bot;
        private static readonly string token = "5076163340:AAF4imWaonoSaWwaNB2L5WXWv0bwGqCNq_c";
        static void Main(string[] args)
        {
            _bot = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(30) };
            Console.WriteLine($"Привет, я {_bot.GetMeAsync().Result.FirstName} и я помогу тебе узнать погоду.");

            _bot.OnMessage += _bot_OnMessage;

            _bot.StartReceiving();
            Console.ReadLine();
            _bot.StopReceiving();
        }

        private static async void _bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message.Type == MessageType.Text)
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, message.Text);
                Console.WriteLine(message.Text);
            }
        }
    }

}


