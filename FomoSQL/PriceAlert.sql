CREATE TABLE [dbo].[PriceAlert]
(
	[SymbolID] INT NOT NULL,
	[UserID] NVARCHAR(450) NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[ThresholdPrice] DECIMAL(6,2) NOT NULL,
	[IsBullTrigger] BIT NOT NULL,
	CONSTRAINT FK_PriceAlert_UserId FOREIGN KEY ([UserId]) REFERENCES AspNetUsers(Id)
);
GO;

CREATE NONCLUSTERED INDEX IX_PriceAlert_FK_UserId ON PriceAlert(UserId);
GO;

