using Core.DMs;

namespace Core
{
    public class MatchData
    {
        public int ID { get; set; }
        public int SummonerID { get; set; }
        public string LeagueMatchID { get; set; }
        public string Champion { get; set; }
        public string GameMode { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public TimeSpan TimeSpentDead { get; set; }
        public bool Victory { get; set; }
        public int YesVotes { get; set; } = 0;
        public int NoVotes { get; set; } = 0;
        public DateTime ImportDateUTC { get; set; } = DateTime.UtcNow;
        public DateTime MatchStartTimeUTC { get; set; }
        public TimeSpan MatchLength { get; set; }

        public virtual ICollection<MatchDataComment> MatchDataComments { get; set; }
        public virtual Summoner Summoner { get; set; }
    }
}