ALTER TABLE [dbo].[T_Game_User]
ADD Is_Active [BIT] CONSTRAINT DF_T_Game_User_Is_Active DEFAULT(0) NOT NULL
GO