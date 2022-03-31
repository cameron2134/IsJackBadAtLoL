using Core;
using Core.DMs;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchDataRequester.Functions
{
    public class WeeklyFeederCalculator
    {
        private readonly LolContext _context;

        public WeeklyFeederCalculator(LolContext lolContext)
        {
            _context = lolContext;
        }

        [FunctionName("WeeklyFeederCalculator")]
        public async Task Run([TimerTrigger("%WeeklyFeederCronSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"WeeklyFeederCalculator started at: {DateTime.Now}");

            var summonerKillDeaths = new List<Tuple<int, int, int>>();

            var lastWeeksMatches = await GetWeeklyMatches();
            foreach (var summonerMatchGroup in lastWeeksMatches)
            {
                var totalKills = 0;
                var totalDeaths = 0;

                foreach (var match in summonerMatchGroup)
                {
                    totalKills += match.Kills;
                    totalDeaths += match.Deaths;
                }

                summonerKillDeaths.Add(new Tuple<int, int, int>(summonerMatchGroup.Key, totalKills, totalDeaths));
            }

            var topFeeder = summonerKillDeaths
                .OrderByDescending(o => o.Item3 - o.Item2)
                .FirstOrDefault();

            var saveStatus = await AddWeeklyFeeder(topFeeder);
            log.LogInformation($"Was feeder addition status successful: {saveStatus}");
        }

        private async Task<IEnumerable<IGrouping<int, MatchData>>> GetWeeklyMatches()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var data = await _context.MatchDatas
                .Include(o => o.Summoner)
                .Where(o => o.Summoner.Active && o.MatchStartTimeUTC >= lastWeek)
                .ToListAsync();

            return data.GroupBy(o => o.SummonerID);
        }

        private async Task<bool> AddWeeklyFeeder(Tuple<int, int, int> topFeeder)
        {
            var weeklyFeeder = new WeeklyFeeder
            {
                SummonerID = topFeeder.Item1,
                TotalKills = topFeeder.Item2,
                TotalDeaths = topFeeder.Item3,
            };

            _context.WeeklyFeeders.Add(weeklyFeeder);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}