using Newtonsoft.Json;

namespace AwqatSalaat.Services.QCH
{
    public class QchPrayData
    {
        public int CityId { get; set; }
        public string Fajr { get; set; }
        [JsonProperty("shrouq")]
        public string Shuruq { get; set; }
        [JsonProperty("thahr")]
        public string Dhuhr { get; set; }
        [JsonProperty("aser")]
        public string Asr { get; set; }
        [JsonProperty("moghreb")]
        public string Maghrib { get; set; }
        [JsonProperty("ishaa")]
        public string Isha { get; set; }
    }
}
