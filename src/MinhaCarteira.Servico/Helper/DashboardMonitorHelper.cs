using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Modelo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Servico.Helper;

public static class DashboardMonitorHelper
{
    public static async Task<IList<DashboardMonitor>> ObterMonitoresDashbaord(
        IDashboardMonitorServico dashboardMonitorServico,
        IMovimentoBancarioServico movimentoBancarioServico,
        ICriterio criterio,
        string mesAno)
    {
        criterio.ItensPorPagina = 1;
        criterio.AdicionarIncludes = false;
        criterio.AdicionarFiltro("Competencia", new FiltroOpcao()
        {
            NomePropriedade = "Competencia",
            Operador = TipoOperadorBusca.Igual,
            Valor = mesAno
        });

        var movResponse = await movimentoBancarioServico.Navegar(criterio);
        var movimentos = movResponse.Dados;

        criterio.GruposFiltro.RemoveAll(r => r.NomeGrupoKey.Equals("Competencia"));
        criterio.Ordenacao = "Titulo";
        criterio.AdicionarIncludes = true;
        criterio.ParametrosInclude = [mesAno];
        var response = await dashboardMonitorServico.Navegar(criterio);

        var tasks = (
            from item in response.Dados
            let filtro = string.IsNullOrWhiteSpace(item.CriterioBuscaMovimentosJson)
                ? new FiltroBase()
                : JsonConvert.DeserializeObject<FiltroBase>(item.CriterioBuscaMovimentosJson)
            select Task.Run(() =>
            {
                item.Movimentos = movimentos
                    .Where(filtro.CompilarFiltro<MovimentoBancario>().Compile())
                    .ToList();
            })).ToList();

        await Task.WhenAll(tasks);

        return response.Dados;
    }

    public static async Task<DashboardResumo> ObterResumo(
        IAgendamentoServico agendamentoServico,
        IContaBancariaServico contaBancariaServico,
        IMovimentoBancarioServico movimentoBancarioServico,
        ICriterio criterio,
        string mesAno)
    {
        criterio.ItensPorPagina = 1;
        criterio.AdicionarIncludes = true;
        criterio.GruposFiltro.Add(
            new GrupoFiltro("Contas", [
                new FiltroOpcao(){
                NomePropriedade = "ExibirNaTelaInicial",
                Operador = TipoOperadorBusca.Igual,
                    Valor = true
            }
            ])
        );
        var respContasBancarias = await contaBancariaServico.Navegar(criterio);

        criterio.GruposFiltro.RemoveAll(r => r.NomeGrupoKey.Equals("Contas"));
        criterio.GruposFiltro.Add(
            new GrupoFiltro("Movimentos", [
                new FiltroOpcao(){
                NomePropriedade = "Competencia",
                Operador = TipoOperadorBusca.Igual,
                Valor = mesAno
            }
            ])
        );
        criterio.GruposFiltro.Add(
            new GrupoFiltro("MovimentosIgnorados", [
                new FiltroOpcao(){
                NomePropriedade = "Categoria.IgnorarMovimentacoes",
                Operador = TipoOperadorBusca.Diferente,
                Valor = true,
            },
            new FiltroOpcao(){
                NomePropriedade = "Pessoa.IgnorarMovimentacoes",
                Operador = TipoOperadorBusca.Diferente,
                Valor = true,
            },
            new FiltroOpcao(){
                NomePropriedade = "CentroClassificacao.IgnorarMovimentacoes",
                Operador = TipoOperadorBusca.Diferente,
                Valor = true,
            },
            ])
        );
        var respMovimentos = await movimentoBancarioServico.Navegar(criterio);

        criterio.GruposFiltro.RemoveAll(r => r.NomeGrupoKey.Equals("Movimentos"));
        criterio.GruposFiltro.RemoveAll(r => r.NomeGrupoKey.Equals("MovimentosIgnorados"));
        criterio.GruposFiltro.Add(
            new GrupoFiltro("AgendamentoParcela", [
                new FiltroOpcao(){
                NomePropriedade = "Data",
                Operador = TipoOperadorBusca.MaiorOuIgual,
                Valor = DateTime.ParseExact(mesAno + "01", "yyyyMMdd", CultureInfo.InvariantCulture)
            },
            new FiltroOpcao(){
                NomePropriedade = "Data",
                Operador = TipoOperadorBusca.MenorOuIgual,
                Valor = DateTime.ParseExact(
                    mesAno + DateTime.DaysInMonth(
                        int.Parse(mesAno[..4]),
                        int.Parse(mesAno.Substring(4, 2)))
                    , "yyyyMMdd",
                    CultureInfo.InvariantCulture)
            }
            ])
        );
        criterio.AdicionarIncludes = true;
        criterio.GruposFiltro
            .FirstOrDefault(w => w.NomeGrupoKey.Equals("Sistema", System.StringComparison.InvariantCultureIgnoreCase))
            .Filtros
            .FirstOrDefault(w => w.NomePropriedade.Equals("ProprietarioId", System.StringComparison.InvariantCultureIgnoreCase))
            .NomePropriedade = "Agendamento.ProprietarioId";

        var respAgendamentos = await agendamentoServico.ContasAVencer(criterio);

        var dashboardResumo = new DashboardResumo()
        {
            AgendamentoParcelas = respAgendamentos.Dados,
            ContasBancarias = respContasBancarias.Dados,
            Movimentos = respMovimentos.Dados
        };

        return dashboardResumo;
    }
}
