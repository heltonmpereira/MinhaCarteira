using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IConciliacaoBancariaServico : IServico<ConciliacaoBancaria, Guid, IConciliacaoBancariaRepositorio>
{
    Task<IRespostaServico<ConciliacaoBancaria>> ObterPorParcelaId(Guid id, bool adicionarIncludes = true);
    Task<IRespostaServico<ConciliacaoBancaria>> CadastrarConciliacao(Guid proprietarioId, Guid[] parcelasId, Guid[] movimentosId);
}