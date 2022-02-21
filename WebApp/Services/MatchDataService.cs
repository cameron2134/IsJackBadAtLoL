using AutoMapper;
using Core.DTOs;
using Microsoft.EntityFrameworkCore;
using Repo;

namespace WebApp.Services
{
    public class MatchDataService
    {
        private readonly LolContext _context;
        private readonly IMapper _mapper;

        public MatchDataService(LolContext lolContext, IMapper mapper)
        {
            _context = lolContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SummonerDTO>> GetSummoners()
        {
            var summoners = await _context.Summoners.AsNoTracking().Where(o => o.Active).ToListAsync();
            var mappedSummoners = _mapper.Map<IEnumerable<SummonerDTO>>(summoners);

            return mappedSummoners;
        }

        public async Task<string> GetSummonerNickname(int summonerID)
        {
            var nickname = await _context.Summoners
                .AsNoTracking()
                .Select(o => new { o.Nickname, o.ID })
                .FirstOrDefaultAsync(o => o.ID == summonerID);

            return (nickname != null) ? nickname.Nickname : "N/A";
        }

        public async Task<IEnumerable<MatchDataDTO>> GetMatchHistory(int summonerID, int numRecsToLoad = 9, int numToSkip = 0)
        {
            var fullMatchHistory = await _context.MatchDatas
                .Include(o => o.MatchType)
                .OrderByDescending(o => o.MatchStartTimeUTC)
                .Where(o => o.SummonerID == summonerID)
                .Skip(numToSkip)
                .Take(numRecsToLoad)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<MatchDataDTO>>(fullMatchHistory);
        }

        public async Task<IEnumerable<MatchDataDTO>> GetWeeklyHighlights(int summonerID, int recsToTake = 3, DateTime? startTime = null)
        {
            if (startTime == null)
                startTime = DateTime.UtcNow.AddDays(-7);

            var topWorstGames = await _context.MatchDatas
                .Include(o => o.MatchType)
                .OrderByDescending(o => Math.Abs(o.Deaths - o.Kills))
                .ThenByDescending(o => o.Deaths)
                .Where(o => o.MatchStartTimeUTC > startTime && o.Deaths > o.Kills && o.SummonerID == summonerID)
                .Take(recsToTake)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<MatchDataDTO>>(topWorstGames);
        }

        public async Task<GlobalStatisticsDTO> GetOverallStatistics(int summonerID)
        {
            var statistics = await _context.GlobalStatistics.FirstOrDefaultAsync(o => o.SummonerID == summonerID);
            return _mapper.Map<GlobalStatisticsDTO>(statistics);
        }

        public async Task<WeeklyFeederDTO> GetWeeklyFeeder()
        {
            var weeklyFeeder = await _context.WeeklyFeeders
                .Include(o => o.Summoner)
                .OrderByDescending(o => o.CalculationDateUTC)
                .FirstOrDefaultAsync();

            return _mapper.Map<WeeklyFeederDTO>(weeklyFeeder);
        }

        public async Task<int> GetTotalSummonerMatches(int summonerID)
        {
            return await _context.MatchDatas
                .Where(o => o.SummonerID == summonerID)
                .CountAsync();
        }
    }
}