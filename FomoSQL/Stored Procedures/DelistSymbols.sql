CREATE PROCEDURE [dbo].[DelistSymbols]
	@tvpSymbolID dbo.IntIdType READONLY,
	@isDelisted BIT
AS
	UPDATE Symbol
	SET	
		Delisted = @isDelisted
	FROM 
		Symbol
	INNER JOIN
		@tvpSymbolID tvpIntId
	ON
		Symbol.Id = tvpIntId.Id;
RETURN 
