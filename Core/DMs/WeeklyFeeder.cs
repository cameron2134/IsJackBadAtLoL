using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DMs
{
    public class WeeklyFeeder
    {
        public int ID { get; set; }
        public int SummonerID { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public DateTime CalculationDateUTC { get; set; } = DateTime.UtcNow;

        public virtual Summoner Summoner { get; set; }
    }
}