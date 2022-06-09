namespace Core.DTOs
{
    public class GlobalStatisticsDTO
    {
        public int ID { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalMatchesTracked { get; set; }
        public TimeSpan TimeSpentDead { get; set; }
    }
}