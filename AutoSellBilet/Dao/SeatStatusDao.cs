using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSellBilet.Dao
{
    public class SeatStatusDao
    {
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public string? status { get; set; }
        public decimal price { get; set; }
        public string? UserName { get; set; }
    }
}
