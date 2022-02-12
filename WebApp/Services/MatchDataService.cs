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

        public async Task<JackMatchesDTO> GetMatchHistory(int numRecsToLoad = 9)
        {
            // this will be paginated
            var fullMatchHistory = await _context.MatchDatas
                .Take(numRecsToLoad)
                .OrderByDescending(o => o.MatchStartTimeUTC)
                .AsNoTracking()
                .ToListAsync();

            var oneWeek = DateTime.UtcNow.AddDays(-7);
            var topWorstGames = await _context.MatchDatas
                .Where(o => o.MatchStartTimeUTC > oneWeek && o.Deaths > o.Kills)
                .OrderByDescending(o => o.Deaths)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            var fullMatchData = new JackMatchesDTO();

            var statistics = await _context.GlobalStatistics.FirstOrDefaultAsync(o => o.ID == 1);
            fullMatchData.Statistics = _mapper.Map<GlobalStatisticsDTO>(statistics);

            fullMatchData.MatchHistory = _mapper.Map<IEnumerable<MatchDataDTO>>(fullMatchHistory);
            fullMatchData.Highlights = _mapper.Map<IEnumerable<MatchDataDTO>>(topWorstGames);
            return fullMatchData;
        }
    }
}