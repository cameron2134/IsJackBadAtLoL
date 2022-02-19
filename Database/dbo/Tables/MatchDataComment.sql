CREATE TABLE [dbo].[MatchDataComment] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [MatchDataID] INT           NOT NULL,
    [Comment]     VARCHAR (150) NOT NULL,
    CONSTRAINT [PK_MatchData_Comment] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MatchData_Comment_MatchData] FOREIGN KEY ([MatchDataID]) REFERENCES [dbo].[MatchData] ([ID])
);

