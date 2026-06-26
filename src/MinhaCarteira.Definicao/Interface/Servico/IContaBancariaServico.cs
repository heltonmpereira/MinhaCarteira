using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IContaBancariaServico : IServico<ContaBancaria, Guid, IContaBancariaRepositorio>
{
    Task<IRespostaServico<bool>> Reativar(Guid id);
    Task<IRespostaServico<bool>> IncrementarPrioridade(Guid id);
    Task<IRespostaServico<bool>> DecrementarPrioridade(Guid id);
}
