namespace Core.DMs
{
    public class GlobalStatistics
    {
        public int ID { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalMatchesTracked { get; set; }
        public TimeSpan TotalTimeSpentDead { get; set; }
    }
}