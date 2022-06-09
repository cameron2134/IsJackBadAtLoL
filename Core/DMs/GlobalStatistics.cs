namespace Core.DMs
{
    public class GlobalStatistics
    {
        public int ID { get; set; }
        public int SummonerID { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalMatchesTracked { get; set; }
        public int SecondsSpentDead { get; set; }

        public virtual Summoner Summoner { get; set; }
    }
}