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

        public DataRequestJob(HttpClient httpClient, LolContext lolContext)
        {
            _httpClient = httpClient;
            _context = lolContext;

            _apiKey = FunctionConfigHelper.GetSetting("LeagueApiKey");
        }

        [FunctionName("DataRequestJob")]
        public async Task Run([TimerTrigger("%CronSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"DataRequestJob started at: {DateTime.Now}");

            var jackPuuid = FunctionConfigHelper.GetSetting("JackPuuid");

            int totalDeaths = 0;
            int numMatchesProcessoed = 0;
            TimeSpan totalTimeSpentDead = new TimeSpan();
            bool dataChanged = false;

            var recentMatches = await GetMatchHistory();
            foreach (var matchID in recentMatches)
            {
                try
                {
                    var deathData = await ProcessMatchHistory(matchID, jackPuuid);
                    if (deathData == null)
                        continue;

                    dataChanged = true;
                    numMatchesProcessoed++;
                    totalDeaths += deathData.Item1;
                    totalTimeSpentDead += deathData.Item2;
                }
                catch (Exception ex)
                {
                    log.LogError($"Could not process match {matchID}: {ex.Message}, {ex.StackTrace}");
                }
            }

            if (dataChanged)
            {
                await UpdateGlobalStatistics(totalDeaths, totalTimeSpentDead, numMatchesProcessoed);

                var numAdded = await _context.SaveChangesAsync();
                log.LogInformation($"Saved {numAdded} new records.");
            }
            else
            {
                log.LogInformation($"No data to save.");
            }
        }

        private async Task<Tuple<int, TimeSpan>> ProcessMatchHistory(string matchID, string jackPuuid)
        {
            if (DoesMatchExist(matchID))
                return null;

            var matchData = await GetMatchData(matchID);
            var jack = matchData.info.participants.FirstOrDefault(o => o.puuid == jackPuuid);

            var data = new MatchData
            {
                LeagueMatchID = matchData.metadata.matchId,
                Champion = jack.championName,
                GameMode = matchData.info.gameMode,
                Kills = jack.kills,
                Deaths = jack.deaths,
                Assists = jack.assists,
                TimeSpentDead = TimeSpan.FromSeconds(jack.totalTimeSpentDead),
                Victory = jack.win,
                MatchStartTimeUTC = GetDateTimeFromInt(matchData.info.gameStartTimestamp),
                MatchLength = TimeSpan.FromSeconds(matchData.info.gameDuration),
            };

            _context.MatchDatas.Add(data);
            return new Tuple<int, TimeSpan>(data.Deaths, data.TimeSpentDead);
        }

        private async Task UpdateGlobalStatistics(int matchDeaths, TimeSpan matchTimeSpentDead, int numMatches)
        {
            var existingRec = await _context.GlobalStatistics.FirstOrDefaultAsync(o => o.ID == 1);
            var recExists = existingRec != null;
            if (!recExists)
            {
                existingRec = new GlobalStatistics
                {
                    ID = 1
                };
            }

            existingRec.TotalTimeSpentDead += matchTimeSpentDead;
            existingRec.TotalDeaths += matchDeaths;
            existingRec.TotalMatchesTracked += numMatches;

            if (recExists)
                _context.GlobalStatistics.Update(existingRec);
            else
                _context.GlobalStatistics.Add(existingRec);
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

        private async Task<LeagueMatchData> GetMatchData(string matchID)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{FunctionConfigHelper.GetSetting("MatchDataEndpoint")}{matchID}"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("X-Riot-Token", _apiKey);

            var response = await _httpClient.SendAsync(request);
            var data = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<LeagueMatchData>(data);
        }

        private async Task<IEnumerable<string>> GetMatchHistory()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{FunctionConfigHelper.GetSetting("MatchHistoryEndpoint")}{FunctionConfigHelper.GetSetting("JackPuuid")}/ids?start=0&count=5"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("X-Riot-Token", _apiKey);

            var response = await _httpClient.SendAsync(request);

            var txt = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<string>>(txt);
        }
    }
}