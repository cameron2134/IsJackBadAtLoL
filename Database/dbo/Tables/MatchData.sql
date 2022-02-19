CREATE TABLE [dbo].[MatchData] (
    [ID]                INT           IDENTITY (1, 1) NOT NULL,
    [SummonerID]        INT           NOT NULL,
    [LeagueMatchID]     VARCHAR (20)  NOT NULL,
    [Champion]          VARCHAR (100) NOT NULL,
    [GameMode]          VARCHAR (50)  NOT NULL,
    [Kills]             INT           NOT NULL,
    [Deaths]            INT           NOT NULL,
    [Assists]           INT           NOT NULL,
    [TimeSpentDead]     TIME (7)      NOT NULL,
    [Victory]           BIT           NOT NULL,
    [YesVotes]          INT           NOT NULL,
    [NoVotes]           INT           NOT NULL,
    [MatchStartTimeUTC] DATETIME      NOT NULL,
    [MatchLength]       TIME (7)      NOT NULL,
    [ImportDateUTC]     DATETIME      NOT NULL,
    CONSTRAINT [PK_MatchData] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MatchData_Summoner] FOREIGN KEY ([SummonerID]) REFERENCES [dbo].[Summoner] ([ID])
);

