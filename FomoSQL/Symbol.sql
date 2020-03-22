CREATE TABLE [dbo].[Symbol]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] VARCHAR(5) NOT NULL,
	[ExchangeName] VARCHAR(10) NOT NULL,
    CONSTRAINT FK_Symbol_ExchangeName FOREIGN KEY (ExchangeName) REFERENCES Exchange(Name),
	CONSTRAINT AK_Symbol_Name_Exchange UNIQUE(Name, ExchangeName),

)
GO;

CREATE NONCLUSTERED INDEX IX_Symbol_FK_ExchangeName ON Portfolio(UserId);
GO;
