using System;
using System.Collections.Generic;
using System.Text;

namespace teleBot
{
    internal class WeatherSiteResponce
    {
        public class WeatherResponse
        {
            public Weather[] weather { get; set; } // id, параметры погоды, описание, иконка
            public Main main { get; set; } // темп, ощущается, max, min, давление, влажность
            public Wind wind { get; set; } // скорость, "наклон" ветра
            public Clouds clouds { get; set; } //% облачности
            public string name { get; set; } //город
            public string message { get; set; } // сообщение ошибки
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
        public class JsonCity
        {
            public string id { get; set; }
            public string name { get; set; }
            public string country { get; set; }
        }
    }
}
