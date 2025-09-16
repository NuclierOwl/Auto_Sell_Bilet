using System;

namespace AutoSellBilet.Dao
{
    internal class SeansDao
    {
        public int Id { get; set; }
        public string? kino_name { get; set; }
        public int zal_id { get; set; }
        public DateTime start_time { set; get; }
        public decimal base_prise {  set; get; }
    }
}
