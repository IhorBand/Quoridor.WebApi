ALTER TABLE [dbo].[T_Game_User]
ADD Is_Able_To_Move [BIT] CONSTRAINT DF_T_Game_User_Is_Able_To_Move DEFAULT(0) NOT NULL
GO