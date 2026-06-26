using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class MovimentoBancarioServico(IMovimentoBancarioRepositorio repositorio, IConciliacaoBancariaServico conciliacaoBancariaServico)
    : BaseServico<MovimentoBancario, Guid, IMovimentoBancarioRepositorio>(repositorio), IMovimentoBancarioServico
{
    public async Task<IRespostaServico<int>> IncluirRange(List<MovimentoBancario> item)
    {
        var inseridosDb = await Repositorio.IncluirRange(item);
        var retorno = new RespostaServico<int>(inseridosDb);
        return retorno;
    }

    public async Task<IRespostaServico<bool>> Transferir(MovimentoBancarioTransferencia item)
    {
        var transferencia = new TransferenciaBancaria(item);
        var transfDb = await Repositorio.CadastrarTransferencia(transferencia).ConfigureAwait(false);
        var retorno = new RespostaServico<bool>(
            transfDb != null,
            transfDb != null
                ? "Transferência registrada com sucesso."
                : "Não foi possível realizar a transferência solicitada.");

        return retorno;
    }

    public async Task<IRespostaServico<int>> ConciliarMovimento(Guid proprietarioId, Guid idMovimento, string idParcelas)
    {
        var resposta = await conciliacaoBancariaServico.CadastrarConciliacao(
            proprietarioId,
            idParcelas
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Guid(s.Replace("'", "").Replace("\"", "")))
                .ToArray(),
            [idMovimento]
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

}
