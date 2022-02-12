namespace Core.DTOs
{
    public class JackMatchesDTO
    {
        public GlobalStatisticsDTO Statistics { get; set; }
        public IEnumerable<MatchDataDTO> MatchHistory { get; set; }
        public IEnumerable<MatchDataDTO> Highlights { get; set; }
    }
}