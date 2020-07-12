CREATE TABLE [dbo].[ExchangeSyncSetting]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[DisableSync] BIT NOT NULL,
	[DisableThresholds] BIT NOT NULL,
	[InsertThresholdPercent] INT NOT NULL,
	[DeleteThresholdPercent] INT NOT NULL,
	[UpdateThresholdPercent] INT NOT NULL,
	[Delimiter] VARCHAR(10),
	[SuffixBlackList] VARCHAR(200),
	[Url] VARCHAR(200),
	[ClientName] VARCHAR(50)
)
