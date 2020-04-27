CREATE TABLE [dbo].[Symbol]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[Ticker] VARCHAR(5) NOT NULL,
	[FullName] VARCHAR(50) NOT NULL,
	[ExchangeName] VARCHAR(10) NOT NULL,
    CONSTRAINT FK_Symbol_ExchangeName FOREIGN KEY (ExchangeName) REFERENCES Exchange(Name),
	CONSTRAINT AK_Symbol_Name_Ticker UNIQUE(Ticker, ExchangeName)
)
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_FK_ExchangeName ON Symbol(ExchangeName);
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_Ticker ON Symbol(Ticker);
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_FullName ON Symbol(FullName);
GO;
