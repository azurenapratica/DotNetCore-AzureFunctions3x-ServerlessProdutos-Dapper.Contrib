USE BaseProdutos
GO

CREATE TABLE dbo.Produtos(
	CodigoBarras VARCHAR(13) NOT NULL,
	Nome VARCHAR(60) NOT NULL,
	Preco NUMERIC (10,4) NOT NULL,
	UltimaAtualizacao DATETIME NOT NULL,
	ObservacaoReajustePreco VARCHAR(100) NULL,
	CONSTRAINT PK_Produtos PRIMARY KEY (CodigoBarras)
)
GO
