CREATE TABLE [dbo].[SingleQuoteData]
(
	[SymbolId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[LastModified] DateTime NOT NULL,
	[Data] VARCHAR(800) NOT NULL,
	CONSTRAINT FK_SingleQuoteData_SymbolId FOREIGN KEY (SymbolId) REFERENCES Symbol(Id)
)
GO;

CREATE NONCLUSTERED INDEX IX_SingleQuoteData_FK_SymbolId ON SingleQuoteData(SymbolId);
GO;