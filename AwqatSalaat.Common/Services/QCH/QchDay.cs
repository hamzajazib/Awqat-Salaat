using Newtonsoft.Json;

namespace AwqatSalaat.Services.QCH
{
    public class QchDay
    {
        [JsonProperty("h")]
        public string HIjri { get; set; }
        [JsonProperty("m")]
        public string Gregorian { get; set; }
        public bool Today { get; set; }
    }
}
