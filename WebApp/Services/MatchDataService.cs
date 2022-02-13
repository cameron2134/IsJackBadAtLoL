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

        public async Task<JackMatchesDTO> GetMatchHistory(int summonerID, int numRecsToLoad = 9)
        {
            // todo - improve this low quality method
            var summoner = await _context.Summoners.FirstOrDefaultAsync(o => o.ID == summonerID);

            var fullMatchHistory = await _context.MatchDatas
                .Where(o => o.SummonerID == summonerID)
                .Take(numRecsToLoad)
                .OrderByDescending(o => o.MatchStartTimeUTC)
                .AsNoTracking()
                .ToListAsync();

            var oneWeek = DateTime.UtcNow.AddDays(-7);
            var topWorstGames = await _context.MatchDatas
                .Where(o => o.MatchStartTimeUTC > oneWeek && o.Deaths > o.Kills && o.SummonerID == summonerID)
                .OrderByDescending(o => o.Deaths)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            var fullMatchData = new JackMatchesDTO
            {
                Nickname = summoner.Nickname
            };

            var statistics = await _context.GlobalStatistics.FirstOrDefaultAsync(o => o.SummonerID == summonerID);
            fullMatchData.Statistics = _mapper.Map<GlobalStatisticsDTO>(statistics);

            fullMatchData.MatchHistory = _mapper.Map<IEnumerable<MatchDataDTO>>(fullMatchHistory);
            fullMatchData.Highlights = _mapper.Map<IEnumerable<MatchDataDTO>>(topWorstGames);
            return fullMatchData;
        }
    }
}