using OpenTelemetry.Trace;

namespace AutoSellBilet.Dao
{
    public class MestoDao
    {
        public int Id { get; set; }
        public int zal_id { get; set; }
        public int row_number { get; set; }
        public int mesto_number { get; set; }
        public string? status { get; set; }

        public string? Textik => $"Ряд {row_number}, Место {mesto_number} ({status})";
    }
}
