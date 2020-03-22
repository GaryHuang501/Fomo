CREATE TABLE [dbo].[PortfolioSymbol]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[PortfolioId] INT NOT NULL,
	[SortOrder] TINYINT NOT NULL,
	CONSTRAINT FK_PortfolioSymbol_PortfolioId FOREIGN KEY ([PortfolioId]) REFERENCES Portfolio(Id)
)
GO;

CREATE NONCLUSTERED INDEX IX_PortfolioSymbol_FK_PortfolioId ON PortfolioSymbol(PortfolioId);
GO;

