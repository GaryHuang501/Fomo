IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'TSLA')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('TSLA', 'Tesla Inc', 'NASDAQ')
END;