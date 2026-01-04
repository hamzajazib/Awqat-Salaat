namespace AwqatSalaat.Services.QCH
{
    public class QchCity
    {
        public int CityId { get; }
        public string Name { get; }
        public string Country { get; }
        public string TimeZone { get; }

        public QchCity(int cityId, string name, string country, string timeZone)
        {
            CityId = cityId;
            Name = name;
            Country = country;
            TimeZone = timeZone;
        }
    }
}
