INSERT INTO Exchange (ID, Name) VALUES
(1, 'NASDAQ'),
(2, 'NYSE'),
(3, 'NYSE ARCA'),
(4, 'NYSE AMERICAN');
GO;

INSERT INTO ExchangeSyncSetting(
	[DisableSync],
	[DisableThresholds],
	[InsertThresholdPercent],
	[DeleteThresholdPercent],
	[UpdateThresholdPercent],
	[Delimiter],
	[SuffixBlackList],
	[Url],
	[ClientName]
) VALUES
(
	0,
	0,
	20,
	10,
	10,
	'|',
	'-,Common Stock',
	'ftp://ftp.nasdaqtrader.com/symboldirectory/nasdaqtraded.txt',
	'NASDAQ'
);
