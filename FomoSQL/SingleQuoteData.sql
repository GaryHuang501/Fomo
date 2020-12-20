CREATE TABLE [dbo].[SingleQuoteData]
(
	[SymbolId] INT PRIMARY KEY,
    [Price] Decimal(19, 2) NOT NULL,
    [High] Decimal(19, 2) NOT NULL,
    [Low] Decimal(19, 2) NOT NULL,
    [Open] Decimal(19, 2) NOT NULL,
    [PreviousClose] Decimal(19, 2),
    [Change] Decimal(19, 4) NOT NULL,
    [ChangePercent] Decimal(19, 4) NOT NULL,
    [Volume] INT NOT NULL,
    [LastUpdated] DateTime NOT NULL,
    [LastTradingDay] DateTime NOT NULL,

	CONSTRAINT FK_SingleQuoteData_SymbolId FOREIGN KEY (SymbolId) REFERENCES Symbol(Id)
)
GO;

CREATE NONCLUSTERED INDEX IX_SingleQuoteData_FK_SymbolId ON SingleQuoteData(SymbolId);
GO;