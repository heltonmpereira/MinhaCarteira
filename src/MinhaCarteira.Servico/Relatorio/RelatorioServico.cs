using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Relatorio.EvolucaoGastos;
using MinhaCarteira.Definicao.Relatorio.EvolucaoSaldo;
using MinhaCarteira.Definicao.Relatorio.EvolucaoSaldoPeriodo;
using MinhaCarteira.Definicao.Relatorio.FluxoCaixa;
using MinhaCarteira.Modelo.Relatorio;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.Servico.Relatorio;

public class RelatorioServico(RelatorioRepositorio repositorio)
{
    public RelatorioRepositorio Repositorio { get; } = repositorio;

    public async Task<IRespostaServico<FluxoCaixa>> FluxoCaixa(int ano, Guid proprietarioId)
    {
        var itens = await Repositorio.FluxoCaixa(ano, proprietarioId).ConfigureAwait(false);

        return new RespostaServico<FluxoCaixa>(itens.Item1);
    }

    public async Task<IRespostaServico<EvolucaoSaldo>> EvolucaoSaldo(int ano, int mes, Guid proprietarioId, Guid? contaBancariaId = null)
    {
        var resultado = await Repositorio.GetEvolucaoSaldo(ano, mes, proprietarioId, contaBancariaId).ConfigureAwait(false);
        return new RespostaServico<EvolucaoSaldo>(resultado);
    }

    public async Task<IRespostaServico<EvolucaoGastos>> EvolucaoGastos(int ano, int mes, Guid proprietarioId, Guid? contaBancariaId = null)
    {
        var resultado = await Repositorio.GetEvolucaoGastos(ano, mes, proprietarioId, contaBancariaId).ConfigureAwait(false);
        return new RespostaServico<EvolucaoGastos>(resultado);
    }

    public async Task<IRespostaServico<EvolucaoSaldoPeriodo>> EvolucaoSaldoPeriodo(DateTime dataInicial, DateTime dataFinal, Guid proprietarioId, Guid? contaBancariaId = null)
    {
        var resultado = await Repositorio.GetEvolucaoSaldoPeriodo(dataInicial, dataFinal, proprietarioId, contaBancariaId).ConfigureAwait(false);
        return new RespostaServico<EvolucaoSaldoPeriodo>(resultado);
    }
}
