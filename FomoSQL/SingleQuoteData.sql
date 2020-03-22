﻿CREATE TABLE [dbo].[SingleQuoteData]
(
	[SymbolId] INT NOT NULL PRIMARY KEY,
	[LastModified] DateTime NOT NULL,
	[Data] VARCHAR(800) NOT NULL,
	CONSTRAINT FK_SingleQuoteData_SymbolId FOREIGN KEY (SymbolId) REFERENCES Symbol(Id)
)
GO;

CREATE NONCLUSTERED INDEX IX_SingleQuoteData_FK_SymbolID ON SingleQuoteData(SymbolId);
GO;