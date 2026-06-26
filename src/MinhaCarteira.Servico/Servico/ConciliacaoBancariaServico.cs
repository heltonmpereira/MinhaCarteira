using System;
using System.Linq;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class ConciliacaoBancariaServico(IConciliacaoBancariaRepositorio repositorio)
    : BaseServico<ConciliacaoBancaria, Guid, IConciliacaoBancariaRepositorio>(repositorio), IConciliacaoBancariaServico
{
    public async Task<IRespostaServico<ConciliacaoBancaria>> ObterPorParcelaId(Guid id, bool adicionarIncludes = true)
    {
        var item = await Repositorio.ObterPorParcelaId(id).ConfigureAwait(false);
        var retorno = new RespostaServico<ConciliacaoBancaria>(item)
        {
            BemSucedido = item != null,
            Mensagem = item != null
                ? "Conciliação localizada pela parcela com sucesso."
                : "Falha ao localizar a conciliação partir da parcela especificada."
        };

        return retorno;
    }

    public async Task<IRespostaServico<ConciliacaoBancaria>> CadastrarConciliacao(Guid proprietarioId, Guid[] parcelasId, Guid[] movimentosId)
    {
        var conciliacaoMovimentos = movimentosId
            .Select(s => new ConciliacaoBancariaMovimento { MovimentoBancarioId = s })
            .ToList();
        var conciliacaoParcelas = parcelasId
            .Select(s => new ConciliacaoBancariaAgendamentoParcela { AgendamentoParcelaId = s })
            .ToList();

        var conciliacao = new ConciliacaoBancaria(conciliacaoMovimentos, conciliacaoParcelas)
        {
            ProprietarioId = proprietarioId
        };
        return await base.Incluir(conciliacao);
    }
}