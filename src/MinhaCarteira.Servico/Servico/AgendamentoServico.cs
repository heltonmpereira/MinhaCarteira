using ChoETL;
using Dates.Recurring;
using Dates.Recurring.Type;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Helper;
using MinhaCarteira.Servico.Servico.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Servico.Servico;

public class AgendamentoServico(
    IAgendamentoRepositorio repositorio,
    IAgendamentoParcelaRepositorio agendParcelaRepositorio,
    IConciliacaoBancariaServico conciliasBancariaServico,
    IDashboardMonitorServico dashboardMonitorServico,
    IMovimentoBancarioServico movimentoBancarioServico) :
    BaseServico<Agendamento, Guid, IAgendamentoRepositorio>(repositorio), IAgendamentoServico
{
    private readonly IDashboardMonitorServico _dashboardMonitorServico = dashboardMonitorServico;
    private readonly IMovimentoBancarioServico _movimentoBancarioServico = movimentoBancarioServico;
    private readonly IConciliacaoBancariaServico _conciliasBancariaServico = conciliasBancariaServico;
    private readonly IAgendamentoParcelaRepositorio _agendParcelaRepositorio = agendParcelaRepositorio;

    private static RecurrenceType ObterRecorrenciaBuilder(Agendamento agend)
    {
        var mfi = new System.Globalization.DateTimeFormatInfo();
        var strMonthName = mfi.GetMonthName(agend.DataInicial.Month);
        var mesEnum = Enum
            .GetValues(typeof(Month))
            .Cast<Enum>()
            .FirstOrDefault(m => m.ToString().Equals(strMonthName, StringComparison.CurrentCultureIgnoreCase));
        var mes = mesEnum != null ? (Month)mesEnum : Month.JANUARY;

        return agend.TipoRecorrencia switch
        {
            TipoRecorrencia.Semanal => Recurs
                .Starting(agend.DataInicial)
                .Every(agend.Intervalo)
                .Weeks()
                .FirstDayOfWeek(DayOfWeek.Monday)
                .OnDay(agend.DataInicial.DayOfWeek)
                .Ending(agend.DataInicial.AddYears(60))
                .Build(),

            TipoRecorrencia.Mensal => Recurs
                .Starting(agend.DataInicial)
                .Every(agend.Intervalo)
                .Months()
                .OnDay(agend.DataInicial.Day)
                .Ending(agend.DataInicial.AddYears(60))
                .Build(),

            TipoRecorrencia.Anual => Recurs
                .Starting(agend.DataInicial)
                .Every(agend.Intervalo)
                .Years()
                .OnDay(agend.DataInicial.Day)
                .OnMonths(mes)
                .Ending(agend.DataInicial.AddYears(60))
                .Build(),

            _ => Recurs
                .Starting(agend.DataInicial)
                .Every(agend.Intervalo)
                .Days()
                .Ending(agend.DataInicial.AddYears(20))
                .Build()
        };
    }
    private static Agendamento GerarParcelas(Agendamento agend)
    {
        var numeroParcela = 0;
        var recorrencia = ObterRecorrenciaBuilder(agend);
        var data = agend.DataInicial;
        DateTime? proximo;

        do
        {
            numeroParcela++;
            agend.AdicionarParcela(data, numeroParcela);

            proximo = recorrencia.Next(data);
            if (proximo.HasValue)
                data = new DateTime(
                    proximo.Value.Year,
                    proximo.Value.Month,
                    proximo.Value.Day);

            switch (agend.TipoParcelas)
            {
                case TipoParcelas.Parcelada:
                    if (agend.Parcelas.Count >= agend.QuantidadeParcelas)
                        proximo = null;

                    break;
                case TipoParcelas.Recorrente:
                    if (proximo >= agend.DataInicial.AddYears(5))
                        proximo = null;

                    break;
                case TipoParcelas.Unica:
                default:
                    proximo = null;

                    break;
            }
        } while (proximo != null);

        return agend;
    }
    public override async Task<IRespostaServico<Agendamento>> Incluir(Agendamento item)
    {
        item = GerarParcelas(item);
        var itemDb = await base.Incluir(item).ConfigureAwait(false);
        item.Parcelas.Clear();
        return itemDb;
    }

    public async Task<IRespostaPaginadaServico<AgendamentoParcela>> ContasAVencer(ICriterio filtro)
    {
        var itens = await _agendParcelaRepositorio.Navegar(filtro).ConfigureAwait(false);
        return MontarRespostaPaginada(itens, filtro);
    }
    public async Task<IRespostaServico<AgendamentoParcela>> ObterParcelaPorId(Guid id)
    {
        var item = await _agendParcelaRepositorio.ObterPorId(id).ConfigureAwait(false);

        var retorno = new RespostaServico<AgendamentoParcela>(item)
        {
            BemSucedido = item != null,
            Mensagem = item != null
                ? "Registro localizado com sucesso."
                : "Falha ao localizar o registro solicitado."
        };

        return retorno;
    }
    public async Task<IRespostaServico<AgendamentoParcela>> AlterarParcela(AgendamentoParcela parcela)
    {
        AgendamentoParcela itemDb;
        if (parcela.AlterarAPenasEstaParcela)
            itemDb = await _agendParcelaRepositorio.Alterar(parcela).ConfigureAwait(false);
        else
        {
            var agend = await Repositorio.ObterPorId(parcela.AgendamentoId, true, ["include=incluirParcelas;"]);
            agend.AtualizarParcelasEmAberto(parcela);
            await Repositorio.AtualizarAgendamento(agend).ConfigureAwait(false);
            itemDb = parcela;
        }

        var retorno = new RespostaServico<AgendamentoParcela>(itemDb)
        {
            BemSucedido = itemDb != null,
            Mensagem = itemDb != null
                ? "Registro alterado com sucesso."
                : "Falha ao alterar o registro solicitado."
        };

        return retorno;
    }
    public async Task<IRespostaServico<int>> DeletarParcela(Guid idParcela)
    {
        var itemDb = await _agendParcelaRepositorio.Deletar(idParcela).ConfigureAwait(false);
        var retorno = new RespostaServico<int>(itemDb)
        {
            BemSucedido = itemDb > 0,
            Mensagem = itemDb > 0
                ? "Registro removido com sucesso."
                : "Falha ao remover o registro solicitado."
        };

        return retorno;
    }
    public async Task<IRespostaServico<AgendamentoParcela>> PagarParcela(AgendamentoParcela parcela)
    {
        var itemDb = await _agendParcelaRepositorio.ObterPorId(parcela.Id).ConfigureAwait(false);
        //itemDb.EstahPaga = true;
        itemDb.ValorPago = parcela.Valor;
        itemDb.DataPagamento = parcela.Data;
        itemDb.AlterarAPenasEstaParcela = true;

        return await AlterarParcela(itemDb);
    }
    public async Task<IRespostaServico<int>> RestaurarCondicaoParcela(Guid idParcela)
    {
        var itemDb = await _agendParcelaRepositorio.ObterPorId(idParcela).ConfigureAwait(false);
        itemDb.AlterarAPenasEstaParcela = true;

        if (itemDb.ConciliacaoBancariaAgendamentoParcelaId == null)
        {
            //itemDb.EstahPaga = false;
            itemDb.ValorPago = null;
            itemDb.DataPagamento = null;
            var alteracao = await AlterarParcela(itemDb);

            return new RespostaServico<int>()
            {
                BemSucedido = alteracao.Dados != null,
                Mensagem = alteracao.Dados != null
                    ? "Condição da parcela restaurado com sucesso."
                    : "Falha ao restaurar a condição da parcela."
            };
        }

        var conciliacao = await _conciliasBancariaServico
            .ObterPorParcelaId((Guid)itemDb.ConciliacaoBancariaAgendamentoParcelaId);
        return await _conciliasBancariaServico.Deletar(conciliacao.Dados.Id);
    }
    public async Task<IRespostaServico<int>> ConciliarParcela(Guid proprietarioId, Guid idParcela, string idMovimentos)
    {
        var resposta = await _conciliasBancariaServico.CadastrarConciliacao(
            proprietarioId,
            [idParcela],
            idMovimentos
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Guid(s.Replace("'", "").Replace("\"", "")))
                .ToArray()
            );

        var retorno = new RespostaServico<int>
        {
            BemSucedido = resposta.BemSucedido,
            Mensagem = resposta.BemSucedido
                ? "Parcela conciliada com sucesso"
                : "Falha ao conciliar a parcela."
        };

        return retorno;
    }

    public async Task<IRespostaServico<bool>> ConciliarParcelasAPartirDashboardMonitor(string mesANo)
    {
        try
        {
            var monitores = await DashboardMonitorHelper.ObterMonitoresDashbaord(
                _dashboardMonitorServico,
                _movimentoBancarioServico,
                new FiltroBase(),
                mesANo);

            foreach (var monitor in monitores)
            {
                var parcelas = monitor.Agendamento.Parcelas
                    .Where(w => w.Data.ToString("yyyyMM") == mesANo)
                    .ToList();

                if (parcelas.Count == 1 && monitor.Movimentos.Count != 0)
                {
                    var parcela = parcelas.FirstOrDefault();
                    parcela.ValorPago = monitor.Movimentos.Sum(mov => mov.ValorReal);
                    parcela.DataPagamento = monitor.Movimentos.Max(mov => mov.DataMovimento);
                    var resultadoPagamento = await _agendParcelaRepositorio.BaixarParcela(parcela);

                    if (resultadoPagamento)
                    {
                        var resultadoConciliacao = await ConciliarParcela(
                            monitor.Agendamento.ProprietarioId,
                            parcela.Id,
                            string.Join(',', monitor.Movimentos.Select(s => s.Id).ToArray()));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
        throw new NotImplementedException();
    }

    public async Task<IRespostaServico<Agendamento>> EstenderParcelasPor5Anos(Guid idAgendamento)
    {
        // Obter o agendamento com as parcelas
        var agend = await Repositorio.ObterPorId(idAgendamento, true, ["include=incluirParcelas;"]);

        if (agend == null)
        {
            return new RespostaServico<Agendamento>(null)
            {
                BemSucedido = false,
                Mensagem = "Agendamento não encontrado."
            };
        }

        // Encontrar a última parcela para começar de lá
        var ultimaParcela = agend.Parcelas.OrderByDescending(p => p.Data).FirstOrDefault();
        if (ultimaParcela == null)
        {
            // Se não tem nenhuma parcela, gerar do início
            agend = GerarParcelas(agend);
        }
        else
        {
            // Gerar mais parcelas começando da última data
            var numeroParcela = ultimaParcela.NumeroParcela;
            var recorrencia = ObterRecorrenciaBuilder(agend);
            var data = ultimaParcela.Data;
            DateTime? proximo = recorrencia.Next(data);

            // Data final: 5 anos após a última parcela
            var dataFinalExtensao = ultimaParcela.Data.AddYears(5);

            do
            {
                if (proximo.HasValue && proximo <= dataFinalExtensao)
                {
                    data = new DateTime(
                        proximo.Value.Year,
                        proximo.Value.Month,
                        proximo.Value.Day);
                    numeroParcela++;
                    agend.AdicionarParcela(data, numeroParcela);
                    proximo = recorrencia.Next(data);
                }
                else
                {
                    proximo = null;
                }

            } while (proximo != null);
        }

        // Salvar o agendamento com as novas parcelas
        await Repositorio.AtualizarAgendamento(agend);

        return new RespostaServico<Agendamento>(agend)
        {
            BemSucedido = true,
            Mensagem = "Parcelas estendidas com sucesso."
        };
    }

    public override async Task<IRespostaServico<int>> Deletar(Guid id)
    {
        var agend = await Repositorio.ObterPorId(id, true, ["include=incluirParcelas;"]);
        var idParcelas = agend.Parcelas
            .Where(w => w.DataPagamento == null)
            .Select(s => s.Id)
            .ToArray();

        var linhasAfetadas = await _agendParcelaRepositorio.DeletarRange(idParcelas);

        var retorno = new RespostaServico<int>(linhasAfetadas)
        {
            BemSucedido = linhasAfetadas > 0,
            Mensagem = linhasAfetadas > 0
                ? $"{linhasAfetadas} registro removido com sucesso."
                : "Falha ao remover os registros solicitados."
        };

        return retorno;
    }

}
