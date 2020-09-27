CREATE TABLE dbo.HistoricoDolar(
	Id INT IDENTITY(1,1) NOT NULL,
	CodReferencia VARCHAR(50) NOT NULL,
	DataReferencia VARCHAR(20) NOT NULL,
	Valor NUMERIC (10,4) NOT NULL,
	VlTimestamp TIMESTAMP NOT NULL,
	CONSTRAINT PK_HistoricoDolar PRIMARY KEY (Id),
	CONSTRAINT UK_HistoricoDolar_CodReferencia UNIQUE (CodReferencia)
)
GO