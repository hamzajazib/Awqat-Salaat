using Newtonsoft.Json;

namespace AwqatSalaat.Services.QCH
{
    public class QchCalendar
    {
        public string Year { get; set; }
        [JsonProperty("days")]
        public QchMonth[] Months { get; set; }
    }
}
