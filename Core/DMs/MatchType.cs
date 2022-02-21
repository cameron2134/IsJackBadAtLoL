namespace Core.DMs
{
    public class MatchType
    {
        public int ID { get; set; }
        public string Type { get; set; }

        public virtual ICollection<MatchData> MatchDatas { get; set; }
    }
}