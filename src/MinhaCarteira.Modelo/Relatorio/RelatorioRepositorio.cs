using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Relatorio.EvolucaoGastos;
using MinhaCarteira.Definicao.Relatorio.EvolucaoSaldo;
using MinhaCarteira.Definicao.Relatorio.EvolucaoSaldoPeriodo;
using MinhaCarteira.Definicao.Relatorio.FluxoCaixa;
using MinhaCarteira.Modelo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Relatorio;

public class RelatorioRepositorio(RelatorioContext contexto, MinhaCarteiraContext carteiraContexto)
{
    protected RelatorioContext Contexto { get; } = contexto;
    protected MinhaCarteiraContext CarteiraContexto { get; } = carteiraContexto;

    public async Task<Tuple<FluxoCaixa, int>> FluxoCaixa(int ano, Guid proprietarioId)
    {
        string cmdConsulta = @$"
SELECT 
  origem, 
  CategoriaPaiNome, 
  CategoriaNome, 
  ""Janeiro"", 
  ""Fevereiro"", 
  ""Março"", 
  ""Abril"", 
  ""Maio"", 
  ""Junho"", 
  ""Julho"", 
  ""Agosto"", 
  ""Setembro"", 
  ""Outubro"", 
  ""Novembro"", 
  ""Dezembro"", 
  ""Total""
FROM 
  crosstab(
    $$ 
    SELECT 
      tmp.""rowid"", 
      tmp.""origem"", 
      tmp.""categoriapainome"", 
      tmp.""categorianome"", 
      tmp.""competencia"", 
      SUM(tmp.""valor"") AS valor 
    From 
      (
        SELECT 
          (
            'Realizado::' || COALESCE(ctgPai.""Nome"", 'Sem Pai') || '::' || ctg.""Nome""
          ) AS rowid, 
          'Realizado' AS origem, 
          COALESCE(ctgPai.""Nome"", 'Sem Pai') AS CategoriaPaiNome, 
          ctg.""Nome""AS CategoriaNome, 
          COALESCE(
            CASE to_char(
              to_date(
                mov.""Competencia""|| '01', 'YYYYMMDD'
              ), 
              'MM'
            ) WHEN '01' THEN 'Janeiro' WHEN '02' THEN 'Fevereiro' WHEN '03' THEN 'Março' WHEN '04' THEN 'Abril' WHEN '05' THEN 'Maio' WHEN '06' THEN 'Junho' WHEN '07' THEN 'Julho' WHEN '08' THEN 'Agosto' WHEN '09' THEN 'Setembro' WHEN '10' THEN 'Outubro' WHEN '11' THEN 'Novembro' WHEN '12' THEN 'Dezembro' END, 
            'Total'
          ) AS competencia, 
          ROUND(
            CASE WHEN mov.""TipoMovimento""= 'Debito' THEN mov.""Valor""* -1 WHEN mov.""TipoMovimento""= 'Credito' THEN mov.""Valor""END, 
            2
          ) AS valor 
        FROM 
         ""MovimentoBancario""mov 
          INNER JOIN""Categoria""ctg ON ctg.""Id""= mov.""CategoriaId""
          LEFT JOIN""Categoria""ctgPai ON ctgPai.""Id""= ctg.""CategoriaPaiId""
        WHERE 
          mov.""Deletado""= FALSE 
          AND mov.""ProprietarioId""= '{proprietarioId}' 
          AND SUBSTRING(mov.""Competencia"", 1, 4) = '{ano}' 
          AND ctg.""IgnorarMovimentacoes""= FALSE 
        UNION ALL 
        SELECT 
          (
            'Realizado::' || COALESCE(ctgPai.""Nome"", 'Sem Pai') || '::' || ctg.""Nome""
          ) AS rowid, 
          'Realizado', 
          COALESCE(ctgPai.""Nome"", 'Sem Pai'), 
          ctg.""Nome"", 
          'Total', 
          ROUND(
            SUM(
              CASE WHEN mov.""TipoMovimento""= 'Debito' THEN mov.""Valor""* -1 WHEN mov.""TipoMovimento""= 'Credito' THEN mov.""Valor""END
            ), 
            2
          ) 
        FROM 
         ""MovimentoBancario""mov 
          INNER JOIN""Categoria""ctg ON ctg.""Id""= mov.""CategoriaId""
          LEFT JOIN""Categoria""ctgPai ON ctgPai.""Id""= ctg.""CategoriaPaiId""
        WHERE 
          mov.""Deletado""= FALSE 
          AND mov.""ProprietarioId""= '{proprietarioId}' 
          AND SUBSTRING(mov.""Competencia"", 1, 4) = '{ano}' 
          AND ctg.""IgnorarMovimentacoes""= FALSE 
        GROUP BY 
          ctgPai.""Nome"", 
          ctg.""Nome""
      ) as tmp 
    group by 
      tmp.""rowid"", 
      tmp.""origem"", 
      tmp.""categoriapainome"", 
      tmp.""categorianome"", 
      tmp.""competencia""
	  
	  $$, 
      $$ VALUES 
      ('Janeiro'), 
      ('Fevereiro'), 
      ('Março'), 
      ('Abril'), 
      ('Maio'), 
      ('Junho'), 
      ('Julho'), 
      ('Agosto'), 
      ('Setembro'), 
      ('Outubro'), 
      ('Novembro'), 
      ('Dezembro'), 
      ('Total') $$
  ) AS realizado(
    rowid text, origem text, CategoriaPaiNome text, 
    CategoriaNome text,""Janeiro""numeric, 
   ""Fevereiro""numeric,""Março""numeric, 
   ""Abril""numeric,""Maio""numeric, 
   ""Junho""numeric,""Julho""numeric, 
   ""Agosto""numeric,""Setembro""numeric, 
   ""Outubro""numeric,""Novembro""numeric, 
   ""Dezembro""numeric,""Total""numeric
  ) 
  
UNION 

SELECT 
  origem, 
  CategoriaPaiNome, 
  CategoriaNome, 
  ""Janeiro"", 
  ""Fevereiro"", 
  ""Março"", 
  ""Abril"", 
  ""Maio"", 
  ""Junho"", 
  ""Julho"", 
  ""Agosto"", 
  ""Setembro"", 
  ""Outubro"", 
  ""Novembro"", 
  ""Dezembro"", 
  ""Total""
FROM 
  crosstab(
    $$ 
    SELECT 
      tmp.""rowid"", 
      tmp.""origem"", 
      tmp.""categoriapainome"", 
      tmp.""categorianome"", 
      tmp.""competencia"", 
      SUM(tmp.""valor"") AS valor 
    From 
      (
        SELECT 
          (
            'Planejado::' || COALESCE(ctgPai.""Nome"", 'Sem Pai') || '::' || ctg.""Nome""
          ) AS rowid, 
          'Planejado' AS origem, 
          COALESCE(ctgPai.""Nome"", 'Sem Pai') AS CategoriaPaiNome, 
          ctg.""Nome""AS CategoriaNome, 
          COALESCE(
            case to_char(parcela.""Data"", 'MM') when '01' THEN 'Janeiro' WHEN '02' THEN 'Fevereiro' WHEN '03' THEN 'Março' WHEN '04' THEN 'Abril' WHEN '05' THEN 'Maio' WHEN '06' THEN 'Junho' WHEN '07' THEN 'Julho' WHEN '08' THEN 'Agosto' WHEN '09' THEN 'Setembro' WHEN '10' THEN 'Outubro' WHEN '11' THEN 'Novembro' WHEN '12' THEN 'Dezembro' end, 
            'Total'
          ) AS competencia, 
          ROUND(
            CASE WHEN agend.""Tipo""= 'Debito' THEN parcela.""Valor""* -1 WHEN agend.""Tipo""= 'Credito' THEN parcela.""Valor""END, 
            2
          ) AS valor 
        FROM 
         ""Agendamento""agend 
          INNER JOIN""AgendamentoParcela""parcela ON agend.""Id""= parcela.""AgendamentoId""
          INNER JOIN""Categoria""ctg ON ctg.""Id""= agend.""CategoriaId""
          LEFT JOIN""Categoria""ctgPai ON ctgPai.""Id""= ctg.""CategoriaPaiId""
        WHERE 
          agend.""Deletado""= FALSE 
          AND parcela.""Deletado""= FALSE 
          AND to_char(parcela.""Data"", 'YYYY') = '{ano}' 
          AND agend.""ProprietarioId""= '{proprietarioId}' 
          AND ctg.""IgnorarMovimentacoes""= FALSE 
        UNION 
        SELECT 
          (
            'Planejado::' || COALESCE(ctgPai.""Nome"", 'Sem Pai') || '::' || ctg.""Nome""
          ) AS rowid, 
          'Planejado', 
          COALESCE(ctgPai.""Nome"", 'Sem Pai'), 
          ctg.""Nome"", 
          'Total', 
          ROUND(
            SUM(
              CASE WHEN agend.""Tipo""= 'Debito' THEN parcela.""Valor""* -1 WHEN agend.""Tipo""= 'Credito' THEN parcela.""Valor""END
            ), 
            2
          ) 
        FROM 
         ""Agendamento""agend 
          INNER JOIN""AgendamentoParcela""parcela ON agend.""Id""= parcela.""AgendamentoId""
          INNER JOIN""Categoria""ctg ON ctg.""Id""= agend.""CategoriaId""
          LEFT JOIN""Categoria""ctgPai ON ctgPai.""Id""= ctg.""CategoriaPaiId""
        WHERE 
          agend.""Deletado""= FALSE 
          AND parcela.""Deletado""= FALSE 
          AND to_char(parcela.""Data"", 'YYYY') = '{ano}' 
          AND agend.""ProprietarioId""= '{proprietarioId}' 
          AND ctg.""IgnorarMovimentacoes""= FALSE 
        GROUP BY 
          ctgPai.""Nome"", 
          ctg.""Nome""
      ) as tmp 
    group by 
      tmp.""rowid"", 
      tmp.""origem"", 
      tmp.""categoriapainome"", 
      tmp.""categorianome"", 
      tmp.""competencia""$$, 
      $$ 
    VALUES 
      ('Janeiro'), 
      ('Fevereiro'), 
      ('Março'), 
      ('Abril'), 
      ('Maio'), 
      ('Junho'), 
      ('Julho'), 
      ('Agosto'), 
      ('Setembro'), 
      ('Outubro'), 
      ('Novembro'), 
      ('Dezembro'), 
      ('Total') $$
  ) AS planejado(
    rowid text, origem text, CategoriaPaiNome text, 
    CategoriaNome text,""Janeiro""numeric, 
   ""Fevereiro""numeric,""Março""numeric, 
   ""Abril""numeric,""Maio""numeric, 
   ""Junho""numeric,""Julho""numeric, 
   ""Agosto""numeric,""Setembro""numeric, 
   ""Outubro""numeric,""Novembro""numeric, 
   ""Dezembro""numeric,""Total""numeric
  ) 
ORDER BY 
  CategoriaPaiNome, 
  CategoriaNome, 
  origem;
";

        var itens = await Contexto.FluxoCaixaItens.FromSqlRaw(cmdConsulta).ToListAsync();
        var fluxoCaixa = new FluxoCaixa(ano, itens);

        return new Tuple<FluxoCaixa, int>(fluxoCaixa, itens.Count);
    }

    public async Task<EvolucaoSaldo> GetEvolucaoSaldo(int ano, int mes, Guid proprietarioId, Guid? contaBancariaId = null)
    {
        var result = new EvolucaoSaldo { Ano = ano, Mes = mes };
        var dataMesAtual = new DateTime(ano, mes, 1);
        var competenciaAtual = dataMesAtual.ToString("yyyyMM");


        // Obtém todas as parcelas agendadas para o mês
        var parcelasQuery = CarteiraContexto.AgendamentoParcelas
            .Include(p => p.Agendamento)
            .Where(p => !p.Deletado &&
                        !p.Agendamento.Deletado &&
                        p.Agendamento.ProprietarioId == proprietarioId &&
                        (p.DataPagamento ?? p.Data).Year == ano &&
                        (p.DataPagamento ?? p.Data).Month == mes);

        if (contaBancariaId.HasValue)
        {
            parcelasQuery = parcelasQuery.Where(p => p.ContaBancariaId == contaBancariaId.Value);
        }

        var parcelas = await parcelasQuery.ToListAsync();

        // Obtém todos os movimentos bancários para o mês
        var movimentosQuery = CarteiraContexto.MovimentosBancarios
            .Where(m => !m.Deletado &&
                       m.Competencia == competenciaAtual && (
                       m.Categoria.IgnorarMovimentacoes == false ||
                       m.CentroClassificacao.IgnorarMovimentacoes == false) &&
                       m.ProprietarioId == proprietarioId);

        if (contaBancariaId.HasValue)
        {
            movimentosQuery = movimentosQuery.Where(m => m.ContaBancariaId == contaBancariaId.Value);
        }

        var movimentos = await movimentosQuery.ToListAsync();

        // Cria um item para cada dia do mês
        var diasNoMes = DateTime.DaysInMonth(ano, mes);
        var saldoAcumuladoPlanejado = 0m;
        var saldoAcumuladoRealizado = 0m;

        for (int dia = 1; dia <= diasNoMes; dia++)
        {
            var data = new DateTime(ano, mes, dia);

            // Calcula o valor planejado para este dia
            var valorPlanejadoDia = parcelas
                .Where(p => p.Data.Date == data.Date)
                .Sum(p => p.Agendamento.Tipo == Definicao.Modelo.TipoMovimento.Credito
                    ? p.ValorPago ?? p.Valor
                    : -p.ValorPago ?? -p.Valor);

            // Calcula o valor realizado para este dia
            var valorRealizadoDia = movimentos
                .Where(m => m.DataMovimento.Date == data.Date)
                .Sum(m => m.ValorReal);

            saldoAcumuladoPlanejado += valorPlanejadoDia;
            saldoAcumuladoRealizado += valorRealizadoDia;

            result.Itens.Add(new EvolucaoSaldoDiario
            {
                Data = data,
                SaldoPlanejado = valorPlanejadoDia,
                SaldoRealizado = valorRealizadoDia,
                SaldoAcumuladoPlanejado = saldoAcumuladoPlanejado,
                SaldoAcumuladoRealizado = saldoAcumuladoRealizado
            });
        }

        return result;
    }

    public async Task<EvolucaoGastos> GetEvolucaoGastos(int ano, int mes, Guid proprietarioId, Guid? contaBancariaId = null)
    {
        var result = new EvolucaoGastos { Ano = ano, Mes = mes };

        var dataMesAtual = new DateTime(ano, mes, 1);
        var dataMesAnterior = dataMesAtual.AddMonths(-1);
        var competenciaAtual = dataMesAtual.ToString("yyyyMM");
        var competenciaAnterior = dataMesAnterior.ToString("yyyyMM");

        result.NomeMesAtual = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);
        result.NomeMesAnterior = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dataMesAnterior.Month);

        // Obtém todos os movimentos de débito para o mês atual (usando DataMovimento)
        var gastosMesAtualQuery = CarteiraContexto.MovimentosBancarios
            .Include(m => m.Categoria)
            .Include(m => m.CentroClassificacao)
            .Where(m => !m.Deletado &&
                       m.ProprietarioId == proprietarioId &&
                       m.Competencia == competenciaAtual && (
                       m.Categoria.IgnorarMovimentacoes == false ||
                       m.CentroClassificacao.IgnorarMovimentacoes == false) &&
                       m.TipoMovimento == Definicao.Modelo.TipoMovimento.Debito);

        if (contaBancariaId.HasValue)
        {
            gastosMesAtualQuery = gastosMesAtualQuery.Where(m => m.ContaBancariaId == contaBancariaId.Value);
        }

        var gastosMesAtual = await gastosMesAtualQuery.ToListAsync();

        // Obtém todos os movimentos de débito para o mês anterior (usando DataMovimento)
        var gastosMesAnteriorQuery = CarteiraContexto.MovimentosBancarios
            .Include(m => m.Categoria)
            .Include(m => m.CentroClassificacao)
            .Where(m => !m.Deletado &&
                       m.ProprietarioId == proprietarioId &&
                       m.Competencia == competenciaAnterior && (
                       m.Categoria.IgnorarMovimentacoes == false ||
                       m.CentroClassificacao.IgnorarMovimentacoes == false) &&
                       m.TipoMovimento == Definicao.Modelo.TipoMovimento.Debito);

        if (contaBancariaId.HasValue)
        {
            gastosMesAnteriorQuery = gastosMesAnteriorQuery.Where(m => m.ContaBancariaId == contaBancariaId.Value);
        }

        var gastosMesAnterior = await gastosMesAnteriorQuery.ToListAsync();

        // Use o mês que tem mais dias para o gráfico
        var diasNoMesAtual = DateTime.DaysInMonth(ano, mes);
        var diasNoMesAnterior = DateTime.DaysInMonth(dataMesAnterior.Year, dataMesAnterior.Month);
        var totalDias = Math.Max(diasNoMesAtual, diasNoMesAnterior);

        var gastosAcumuladosMesAtual = 0m;
        var gastosAcumuladosMesAnterior = 0m;

        for (int dia = 1; dia <= totalDias; dia++)
        {
            // Calcula gastos para este dia no mês atual
            decimal gastosDiaAtual = 0;
            if (dia <= diasNoMesAtual)
            {
                gastosDiaAtual = gastosMesAtual
                    .Where(m => m.DataMovimento.Day == dia)
                    .Sum(m => m.Valor);
                gastosAcumuladosMesAtual += gastosDiaAtual;
            }

            // Calcula gastos para o mesmo dia no mês anterior
            decimal gastosDiaAnterior = 0;
            if (dia <= diasNoMesAnterior)
            {
                gastosDiaAnterior = gastosMesAnterior
                    .Where(m => m.DataMovimento.Day == dia)
                    .Sum(m => m.Valor);
                gastosAcumuladosMesAnterior += gastosDiaAnterior;
            }

            result.Itens.Add(new EvolucaoGastosDiario
            {
                Data = dia <= diasNoMesAtual ? new DateTime(ano, mes, dia) : new DateTime(dataMesAnterior.Year, dataMesAnterior.Month, dia),
                Dia = dia,
                GastosMesAtual = gastosDiaAtual,
                GastosMesAnterior = gastosDiaAnterior,
                GastosAcumuladosMesAtual = dia <= diasNoMesAtual ? gastosAcumuladosMesAtual : null,
                GastosAcumuladosMesAnterior = gastosAcumuladosMesAnterior
            });
        }

        return result;
    }

    public async Task<EvolucaoSaldoPeriodo> GetEvolucaoSaldoPeriodo(DateTime dataInicial, DateTime dataFinal, Guid proprietarioId, Guid? contaBancariaId = null)
    {
        var result = new EvolucaoSaldoPeriodo
        {
            DataInicial = dataInicial,
            DataFinal = dataFinal
        };

        // Obtém as contas bancárias ativas, com todos os movimentos
        var contasQuery = CarteiraContexto.ContasBancarias
            .Include(c => c.Movimentos)
            .Where(c => !c.Deletado && c.ProprietarioId == proprietarioId);
        
        if (contaBancariaId.HasValue)
        {
            contasQuery = contasQuery.Where(c => c.Id == contaBancariaId.Value);
        }
        
        var contas = await contasQuery.ToListAsync();

        // Obtém TODAS as parcelas relevantes (para o gráfico/planejado)
        var contaIds = contas.Select(c => c.Id).ToList();
        var todasParcelas = await CarteiraContexto.AgendamentoParcelas
            .Include(p => p.Agendamento)
            .Where(p => !p.Deletado &&
                        !p.Agendamento.Deletado &&
                        p.Agendamento.ProprietarioId == proprietarioId &&
                        (p.ContaBancariaId == null || contaIds.Contains(p.ContaBancariaId.Value)))
            .ToListAsync();

        // Primeiro, calcula o saldo REAL (apenas movimentos bancários) para cada dia,
        // usando exatamente a mesma lógica do AtualizarSaldos()
        var saldoRealPorDia = new Dictionary<DateTime, decimal>();
        DateTime hoje = DateTime.Today;

        foreach (var conta in contas)
        {
            // Para esta conta, calculamos o saldo começando do ValorSaldoInicial e avançando dia-a-dia
            decimal saldoConta = conta.ValorSaldoInicial;
            DateTime dataSaldoInicial = conta.DataSaldoInicial.Date;

            // Primeiro, precisamos ir do dataSaldoInicial até hoje, e depois do hoje até dataInicial/dataFinal
            var todosMovimentosConta = conta.Movimentos
                .Where(m => !m.Deletado)
                .OrderBy(m => m.DataMovimento)
                .ToList();

            // Vamos percorrer do dataSaldoInicial até a dataFinal (ou hoje, o que for mais longe)
            DateTime dataMaxima = new DateTime[] { dataFinal.Date, hoje.Date }.Max();
            for (DateTime data = dataSaldoInicial; data <= dataMaxima; data = data.AddDays(1))
            {
                // Adiciona os movimentos deste dia
                var movimentosDoDia = todosMovimentosConta
                    .Where(m => m.DataMovimento.Date == data.Date);

                foreach (var mov in movimentosDoDia)
                {
                    saldoConta += mov.ValorReal;
                }

                // Se este dia estiver dentro do nosso período ou for <= hoje, registra
                if ((data >= dataInicial.Date && data <= dataFinal.Date) || data <= hoje.Date)
                {
                    if (!saldoRealPorDia.ContainsKey(data))
                        saldoRealPorDia[data] = 0;
                    
                    saldoRealPorDia[data] += saldoConta;
                }
            }
        }

        // Agora, calculamos o saldo inicial total no dataInicial
        DateTime primeiroDia = dataInicial.Date;
        decimal saldoInicialTotal;
        if (saldoRealPorDia.ContainsKey(primeiroDia.AddDays(-1)))
        {
            saldoInicialTotal = saldoRealPorDia[primeiroDia.AddDays(-1)];
        }
        else
        {
            // Se não temos o dia anterior, calculamos o saldo inicial do primeiro dia
            saldoInicialTotal = contas.Sum(c => c.ValorSaldoInicial);
            
            // Adiciona todos os movimentos do DataSaldoInicial até o dia anterior ao primeiroDia
            foreach (var conta in contas)
            {
                var movimentosAteDiaAnterior = conta.Movimentos
                    .Where(m => !m.Deletado &&
                               m.DataMovimento.Date > conta.DataSaldoInicial.Date &&
                               m.DataMovimento.Date < primeiroDia);
                
                saldoInicialTotal += movimentosAteDiaAnterior.Sum(m => m.ValorReal);
            }
        }
        result.SaldoInicialTotal = saldoInicialTotal;

        // Agora preenchemos o resultado com todos os dias
        decimal saldoAcumulado = saldoInicialTotal;
        for (DateTime data = dataInicial.Date; data <= dataFinal.Date; data = data.AddDays(1))
        {
            decimal saldoInicialDia = saldoAcumulado;

            // Movimentos realizados no dia (apenas MovimentoBancario)
            var movimentosDoDia = contas.SelectMany(c => c.Movimentos)
                .Where(m => !m.Deletado && m.DataMovimento.Date == data.Date);
            
            decimal movimentosRealizadosDia = movimentosDoDia.Sum(m => m.ValorReal);

            // Movimentos planejados no dia (apenas parcelas, para o gráfico)
            decimal movimentosPlanejadosDia = todasParcelas
                .Where(p => (p.DataPagamento ?? p.Data).Date == data.Date)
                .Sum(p => p.Agendamento.Tipo == Definicao.Modelo.TipoMovimento.Credito
                    ? p.ValorPago ?? p.Valor
                    : -p.ValorPago ?? -p.Valor);

            // Saldo final do dia - SE a data for <= hoje, usa o saldo real que calculamos!
            decimal saldoFinalDia;
            if (data.Date <= hoje.Date && saldoRealPorDia.ContainsKey(data.Date))
            {
                saldoFinalDia = saldoRealPorDia[data.Date];
            }
            else
            {
                // Para datas futuras, usa a lógica do gráfico/planejado
                saldoFinalDia = saldoAcumulado + movimentosRealizadosDia + movimentosPlanejadosDia;
            }

            result.Itens.Add(new EvolucaoSaldoPeriodoDiario
            {
                Data = data,
                SaldoInicial = saldoInicialDia,
                MovimentosRealizados = movimentosRealizadosDia,
                MovimentosPlanejados = movimentosPlanejadosDia,
                SaldoFinal = saldoFinalDia
            });

            saldoAcumulado = saldoFinalDia;
        }

        return result;
    }
}
