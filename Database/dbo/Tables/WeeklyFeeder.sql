CREATE TABLE [dbo].[WeeklyFeeder] (
    [ID]                 INT      IDENTITY (1, 1) NOT NULL,
    [SummonerID]         INT      NOT NULL,
    [TotalKills]         INT      NOT NULL,
    [TotalDeaths]        INT      NOT NULL,
    [CalculationDateUTC] DATETIME NOT NULL,
    CONSTRAINT [PK_WeeklyFeeder] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_WeeklyFeeder_Summoner] FOREIGN KEY ([SummonerID]) REFERENCES [dbo].[Summoner] ([ID])
);

