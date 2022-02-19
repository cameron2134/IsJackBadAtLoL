CREATE TABLE [dbo].[Summoner] (
    [ID]       INT           IDENTITY (1, 1) NOT NULL,
    [PUUID]    VARCHAR (100) NULL,
    [Name]     VARCHAR (20)  NOT NULL,
    [Nickname] VARCHAR (50)  NULL,
    [Active]   BIT           NOT NULL,
    CONSTRAINT [PK_Summoner] PRIMARY KEY CLUSTERED ([ID] ASC)
);

