CREATE PROCEDURE [dbo].[UpdatePortfolioSortOrder]
	@tvpNewSortOrder PortfolioSymbolSortOrderType READONLY, 
	@PortfolioId INT
AS
	UPDATE PortfolioSymbol
	SET	
		SortOrder = newSortOrder.SortOrder
	FROM 
		PortfolioSymbol
	INNER JOIN
		@tvpNewSortOrder newSortOrder
	ON
		PortfolioSymbol.Id = newSortOrder.PortfolioSymbolId;

RETURN;
