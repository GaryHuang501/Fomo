CREATE PROCEDURE [dbo].[UpdateSymbols]
	@tvpUpdateSymbol dbo.UpdateSymbolType READONLY
AS
	UPDATE Symbol
	SET	
		FullName = tvpUpdateSymbol.FullName,
		ExchangeId = tvpUpdateSymbol.ExchangeID
	FROM 
		Symbol
	INNER JOIN
		@tvpUpdateSymbol tvpUpdateSymbol
	ON
		Symbol.Id = tvpUpdateSymbol.Id;
RETURN 
