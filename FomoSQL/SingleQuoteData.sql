CREATE TABLE [dbo].[SingleQuoteData]
(
	[SymbolId] INT PRIMARY KEY,
    [Price] Decimal(19, 2) NOT NULL,
    [Change] Decimal(19, 4) NOT NULL,
    [ChangePercent] Decimal(19, 4) NOT NULL,
    [LastUpdated] DateTime NOT NULL,

	CONSTRAINT FK_SingleQuoteData_SymbolId FOREIGN KEY (SymbolId) REFERENCES Symbol(Id)
)
GO;

CREATE NONCLUSTERED INDEX IX_SingleQuoteData_FK_SymbolId ON SingleQuoteData(SymbolId);
GO;