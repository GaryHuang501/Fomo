CREATE PROCEDURE [dbo].[DelistSymbols]
	@tvpSymbolID dbo.IntIdType READONLY
AS
	UPDATE Symbol
	SET	
		Delisted = 1
	FROM 
		Symbol
	INNER JOIN
		@tvpSymbolID tvpIntId
	ON
		Symbol.Id = tvpIntId.Id;
RETURN 
