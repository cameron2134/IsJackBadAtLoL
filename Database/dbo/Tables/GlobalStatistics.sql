CREATE TABLE [dbo].[GlobalStatistics] (
    [ID]                  INT      IDENTITY (1, 1) NOT NULL,
    [SummonerID]          INT      NOT NULL,
    [TotalMatchesTracked] INT      NOT NULL,
    [TotalDeaths]         INT      NOT NULL,
    [TotalTimeSpentDead]  TIME (7) NOT NULL,
    CONSTRAINT [PK_GlobalStatistics] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_GlobalStatistics_Summoner] FOREIGN KEY ([SummonerID]) REFERENCES [dbo].[Summoner] ([ID])
);

