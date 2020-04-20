CREATE TABLE [dbo].[PortfolioSymbol]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[PortfolioId] INT NOT NULL,
	[SymbolId] INT NOT NULL,
	[SortOrder] TINYINT NOT NULL
	CONSTRAINT FK_PortfolioSymbol_PortfolioId FOREIGN KEY ([PortfolioId]) REFERENCES Portfolio(Id),
	CONSTRAINT FK_PortfolioSymbol_SymbolId FOREIGN KEY (SymbolId) REFERENCES Symbol(Id)
)
GO;

CREATE NONCLUSTERED INDEX IX_PortfolioSymbol_FK_PortfolioId ON PortfolioSymbol(PortfolioId);
GO;

CREATE NONCLUSTERED INDEX IX_PortfolioSymbol_FK_SymbolId ON PortfolioSymbol(SymbolId);
GO;