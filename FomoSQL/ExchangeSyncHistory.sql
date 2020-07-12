CREATE TABLE [dbo].[ExchangeSyncHistory]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[ActionName] VARCHAR(100) NOT NULL,
	[Message] VARCHAR(300) NOT NULL,
	[SymbolsChanged] INT NOT NULL,
	[Error] TEXT NULL,
	[DateCreated] DateTime NOT NULL
)
