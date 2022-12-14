CREATE TABLE [dbo].[T_Game](
	[PK_Game] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_Game_PK_Game DEFAULT NEWID() NOT NULL,
	[Name] [NVARCHAR](60) NOT NULL,
	[Max_Players] INT NOT NULL,
	[Status] INT NOT NULL,
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_Game_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_Game_PK_Game] PRIMARY KEY CLUSTERED
	(
		[PK_Game] ASC
	)
)
GO

CREATE TABLE [dbo].[T_Game_User](
	[PK_Game_User] [UNIQUEIDENTIFIER] CONSTRAINT DF_T_Game_User_PK_Game_User DEFAULT NEWID() NOT NULL,
	[FK_Game] [UNIQUEIDENTIFIER] FOREIGN KEY REFERENCES [dbo].[T_Game]([PK_Game]) NOT NULL,
	[FK_User] [UNIQUEIDENTIFIER] FOREIGN KEY REFERENCES [dbo].[T_User]([PK_User]) NOT NULL,
	[CreatedDateUTC] [DATETIME] CONSTRAINT DF_T_Game_User_CreatedDateUTC DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT [T_Game_User_PK_Game_User] PRIMARY KEY CLUSTERED
	(
		[PK_Game_User] ASC
	)
)
GO


