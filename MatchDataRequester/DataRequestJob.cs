using Core;
using Core.DMs;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatchDataRequester
{
    public class DataRequestJob
    {
        private readonly HttpClient _httpClient;
        private readonly LolContext _context;
        private readonly string _apiKey;
        private readonly ILogger<DataRequestJob> _log;

        public DataRequestJob(HttpClient httpClient, LolContext lolContext, ILogger<DataRequestJob> log)
        {
            _httpClient = httpClient;
            _context = lolContext;
            _log = log;

            _apiKey = FunctionConfigHelper.GetSetting("LeagueApiKey");
        }

        [FunctionName("DataRequestJob")]
        public async Task Run([TimerTrigger("%CronSchedule%", RunOnStartup = true)] TimerInfo myTimer)
        {
            _log.LogInformation($"DataRequestJob started at: {DateTime.Now}");

            await UpdateSummoners();

            var allSummoners = await GetSummoners();
            foreach (var summoner in allSummoners)
            {
                var summonerStatistics = await _context.GlobalStatistics.FirstOrDefaultAsync(o => o.SummonerID == summoner.ID);
                if (summonerStatistics == null)
                {
                    summonerStatistics = new GlobalStatistics { SummonerID = summoner.ID };
                    _context.GlobalStatistics.Add(summonerStatistics);
                }
                else
                    _context.GlobalStatistics.Update(summonerStatistics);

                summonerStatistics = await GetDataForSummoner(summoner, summonerStatistics);
            }

            var recsChanged = await _context.SaveChangesAsync();
            _log.LogInformation($"Saved {recsChanged} records.");
        }

        private async Task<GlobalStatistics> GetDataForSummoner(Summoner summoner, GlobalStatistics summonerStatistics)
        {
            try
            {
                var recentMatches = await SendAPIRequest<IEnumerable<string>>($"{FunctionConfigHelper.GetSetting("MatchHistoryEndpoint")}{summoner.PUUID}/ids?start=0&count=5", "X-Riot-Token", HttpMethod.Get);
                foreach (var matchID in recentMatches)
                {
                    var deathData = await ProcessMatchHistory(matchID, summoner);
                    if (deathData == null)
                        continue;

                    summonerStatistics.TotalMatchesTracked++;
                    summonerStatistics.TotalDeaths += deathData.Item1;
                    summonerStatistics.TotalTimeSpentDead += deathData.Item2;
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Unable to process match history for {summoner.PUUID}: {ex.Message}, {ex.StackTrace}");
            }

            return summonerStatistics;
        }

        private async Task UpdateSummoners()
        {
            var summonersNoPuuid = await _context.Summoners
                .Where(o => o.Active && string.IsNullOrWhiteSpace(o.PUUID))
                .ToListAsync();

            if (!summonersNoPuuid.Any())
            {
                _log.LogInformation("No new summoners have been added that require a PUUID.");
                return;
            }

            foreach (var summoner in summonersNoPuuid)
            {
                _log.LogInformation($"Obtaining PUUID for {summoner.Name}");

                try
                {
                    var leagueSummoner = await SendAPIRequest<LeagueSummoner>(FunctionConfigHelper.GetSetting("SummonerNameEndpoint") + summoner.Name, "X-Riot-Token", HttpMethod.Get);
                    summoner.PUUID = leagueSummoner.puuid;
                }
                catch (Exception ex)
                {
                    _log.LogError($"Failed to obtain PUUID for summoner name {summoner.Name}: {ex.Message}, {ex.StackTrace}");
                }
            }

            var summonersUpdated = await _context.SaveChangesAsync();
            _log.LogInformation($"Updated {summonersUpdated} summoner records.");
        }

        private async Task<IEnumerable<Summoner>> GetSummoners()
        {
            var allSummoners = await _context.Summoners
                .Where(o => o.Active)
                .AsNoTracking()
                .ToListAsync();

            return allSummoners;
        }

        private async Task<Tuple<int, TimeSpan>> ProcessMatchHistory(string matchID, Summoner summoner)
        {
            if (DoesMatchExist(matchID))
                return null;

            var fullLeagueMatch = await SendAPIRequest<LeagueMatchData>(FunctionConfigHelper.GetSetting("MatchDataEndpoint") + matchID, "X-Riot-Token", HttpMethod.Get);
            var participantData = fullLeagueMatch.info.participants.FirstOrDefault(o => o.puuid == summoner.PUUID);

            var matchData = new MatchData
            {
                LeagueMatchID = fullLeagueMatch.metadata.matchId,
                SummonerID = summoner.ID,
                Champion = participantData.championName,
                GameMode = fullLeagueMatch.info.gameMode,
                Kills = participantData.kills,
                Deaths = participantData.deaths,
                Assists = participantData.assists,
                TimeSpentDead = TimeSpan.FromSeconds(participantData.totalTimeSpentDead),
                Victory = participantData.win,
                MatchStartTimeUTC = GetDateTimeFromInt(fullLeagueMatch.info.gameStartTimestamp),
                MatchLength = TimeSpan.FromSeconds(fullLeagueMatch.info.gameDuration),
            };

            _context.MatchDatas.Add(matchData);
            return new Tuple<int, TimeSpan>(matchData.Deaths, matchData.TimeSpentDead);
        }

        private bool DoesMatchExist(string matchID)
        {
            return _context.MatchDatas.FirstOrDefault(o => o.LeagueMatchID == matchID) != null;
        }

        private DateTime GetDateTimeFromInt(long time)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(time).ToUniversalTime();

            return date;
        }

        private async Task<T> SendAPIRequest<T>(string apiUrl, string headerName, HttpMethod httpMethod)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(apiUrl),
                Method = httpMethod,
            };
            //request.Headers.Add("X-Riot-Token", _apiKey);
            request.Headers.Add(headerName, _apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseContent);
        }
    }
}