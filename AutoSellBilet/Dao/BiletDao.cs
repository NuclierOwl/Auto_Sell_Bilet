using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSellBilet.Dao
{
    public class BiletDao
    {
        public int id {  get; set; }
        public int seans_mesta_id { get; set; }
        public Guid zritel_guid { get; set; }
        public string stasus { get; set; }
    }
}
