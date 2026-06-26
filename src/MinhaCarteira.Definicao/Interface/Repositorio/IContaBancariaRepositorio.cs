using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IContaBancariaRepositorio : IRepositorio<ContaBancaria, Guid>
{
    Task<bool> Reativar(Guid id);
    Task<bool> AlterarOrdemContaBancaria(Guid id, int direcao);
    Task<bool> AtualizarSaldos();
}
