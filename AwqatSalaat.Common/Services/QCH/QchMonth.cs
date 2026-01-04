namespace AwqatSalaat.Services.QCH
{
    public class QchMonth
    {
        public string HName { get; set; }
        public string MName { get; set; }
        public int Month { get; set; }
        public int Start { get; set; }
        public QchDay[] Days { get; set; }
    }
}
