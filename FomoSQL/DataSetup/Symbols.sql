IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'TSLA')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('TSLA', 'Tesla Inc', 'NASDAQ')
END;

IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'FB')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('FB', 'Facebook, Inc. Common Stock', 'NASDAQ')
END;

IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'MSFT')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('MSFT', 'Microsoft Corporation', 'NASDAQ')
END;

IF NOT EXISTS(SELECT * FROM Symbol WHERE Ticker = 'JPM')
BEGIN
	INSERT INTO Symbol (Ticker, FullName, ExchangeName) VALUES
	('JPM', 'JPMorgan Chase & Co.', 'NYSE')
END;
