CREATE TABLE [dbo].[PriceAlert]
(
	[PortfolioSymbolId] INT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[ThresholdPrice] DECIMAL(6,2) NOT NULL,
	[IsBullTrigger] BIT NOT NULL,
	CONSTRAINT FK_PriceAlert_UserId FOREIGN KEY ([PortfolioSymbolId]) REFERENCES PortfolioSymbol(Id)
);
GO;

CREATE NONCLUSTERED INDEX IX_PriceAlert_FK_UserId ON PriceAlert(PortfolioSymbolId);
GO;

