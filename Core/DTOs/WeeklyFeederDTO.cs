using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class WeeklyFeederDTO
    {
        public int ID { get; set; }
        public int SummonerID { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
    }
}