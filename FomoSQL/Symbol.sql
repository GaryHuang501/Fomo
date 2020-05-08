CREATE TABLE [dbo].[Symbol]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[Ticker] VARCHAR(5) NOT NULL,
	[FullName] VARCHAR(50) NOT NULL,
	[ExchangeId] INT NOT NULL,
    CONSTRAINT FK_Symbol_ExchangeId FOREIGN KEY (ExchangeId) REFERENCES Exchange(Id),
	CONSTRAINT AK_Symbol_Name_Ticker UNIQUE(Ticker, ExchangeId)
)
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_FK_ExchangeId ON Symbol(ExchangeId);
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_Ticker ON Symbol(Ticker);
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_FullName ON Symbol(FullName);
GO;
