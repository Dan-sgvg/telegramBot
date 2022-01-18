using System;
using System.Collections.Generic;
using System.Text;

namespace teleBot
{
    internal class tokens
    {
        public string BotToken { get; set; }
        public string WeatherSiteUrl { get; set; }
        public string WeatherSiteAppID { get; set; }
        public string CitiesListPath = @"C:\\Users\\danil\\Desktop\\teleBot\\teleBot\\citiesListRU.json";
        public string testJson = "{\"coord\":{\"lon\":37.6156,\"lat\":55.7522},\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"пасмурно\",\"icon\":\"04d\"}],\"base\":\"stations\",\"main\":{\"temp\":260.37,\"feels_like\":254.12,\"temp_min\":258.79,\"temp_max\":261.28,\"pressure\":1028,\"humidity\":85,\"sea_level\":1028,\"grnd_level\":1008},\"visibility\":7601,\"wind\":{ \"speed\":3.14,\"deg\":1,\"gust\":7.4},\"clouds\":{ \"all\":87},\"dt\":1641991992,\"sys\":{ \"type\":2,\"id\":2018597,\"country\":\"RU\",\"sunrise\":1641966757,\"sunset\":1641993747},\"timezone\":10800,\"id\":524901,\"name\":\"Москва\",\"cod\":200}";
        /*{"coord":{"lon":37.6156,"lat":55.7522},"weather":[{"id":804,"main":"Clouds","description":"пасмурно","icon":"04d"}],"base":"stations","main":{"temp":260.37,"feels_like":254.12,"temp_min":258.79,"temp_max":261.28,"pressure":1028,"humidity":85,"sea_level":1028,"grnd_level":1008},"visibility":7601,"wind":{ "speed":3.14,"deg":1,"gust":7.4},"clouds":{ "all":87},"dt":1641991992,"sys":{ "type":2,"id":2018597,"country":"RU","sunrise":1641966757,"sunset":1641993747},"timezone":10800,"id":524901,"name":"Москва","cod":200}
        */
    }
}
