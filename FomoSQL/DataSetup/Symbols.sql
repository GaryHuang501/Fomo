IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'TSLA')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('TSLA', 'Tesla Inc', 'NASDAQ')
END;

IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'JPM')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('JPM', 'JPMorgan Chase & Co.', 'NYSE')
END;