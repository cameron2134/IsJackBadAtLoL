using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DMs
{
    public class Summoner
    {
        public int ID { get; set; }
        public string? PUUID { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<MatchData> MatchDatas { get; set; }
        public virtual ICollection<GlobalStatistics> GlobalStatistics { get; set; }
    }
}