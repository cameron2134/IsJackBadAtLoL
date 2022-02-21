CREATE TABLE [dbo].[MatchType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](15) NOT NULL,
    CONSTRAINT [PK_MatchType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

