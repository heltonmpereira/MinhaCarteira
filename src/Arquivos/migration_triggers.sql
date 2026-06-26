            var trgMontarCaminhoCategoria = @"CREATE OR ALTER TRIGGER [dbo].[trg_montarCaminhoCategoria]
ON [dbo].[Categoria]
AFTER INSERT, UPDATE
AS BEGIN
	with CategoriaCte as(
		select ctg.id
			 , ctg.CategoriaPaiId
			 , ctg.Nome
			 , ctg.Nome NomePai
			 , cast(ctg.Nome as nvarchar(max)) Caminho
		from Categoria ctg
		where ctg.CategoriaPaiId is null

		union all
		select ctg.Id
			 , ctg.CategoriaPaiId
			 , ctg.Nome
			 , v.Nome NomePai
			 , concat(v.Caminho, ' | ', ctg.Nome) Caminho
		from Categoria ctg
		inner join CategoriaCte v on v.Id = ctg.CategoriaPaiId
	)

	update categoria set caminho = ctgView.caminho
	from categoria c
	inner join categoriacte ctgView on c.id = ctgView.id and coalesce(c.Caminho, '') <> ctgView.Caminho 
END";
            migrationBuilder.Sql(trgMontarCaminhoCategoria);

            var trgAtualizarSaldoContaBancaria = @"CREATE OR ALTER TRIGGER [dbo].[trg_atualizarSaldoContaBancaria]
ON [dbo].[MovimentoBancario]
AFTER INSERT, UPDATE
AS BEGIN
	update ContaBancaria
	   set ValorSaldoAtual = ValorSaldoAtual + isnull((case when i.TipoMovimento = 1 then i.Valor else i.Valor * -1 end), 0)
	from ContaBancaria conta
	inner join inserted i on i.ContaBancariaId = conta.Id and i.DataMovimento > conta.DataSaldoInicial
END";
            migrationBuilder.Sql(trgAtualizarSaldoContaBancaria);

