using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;


namespace teleBot
{
    internal class Program
    {
        private static Bot _bot;
        static void Main(string[] args)
        {
            try
            {
                _bot = new Bot();
                Console.ReadLine();
            }
            finally
            {
                if (_bot != null)
                {
                    _bot.Dispose();
                }
            }
        }
    }

}


