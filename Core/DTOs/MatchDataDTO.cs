using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class MatchDataDTO
    {
        public int ID { get; set; }
        public string Champion { get; set; }
        public string GameMode { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public TimeSpan TimeSpentDead { get; set; }
        public bool Victory { get; set; }
        public int YesVotes { get; set; }
        public int NoVotes { get; set; }
        public TimeSpan MatchLength { get; set; }
        public DateTime MatchStartTimeUTC { get; set; }

        public IEnumerable<MatchDataCommentDTO> Comments { get; set; }
    }
}