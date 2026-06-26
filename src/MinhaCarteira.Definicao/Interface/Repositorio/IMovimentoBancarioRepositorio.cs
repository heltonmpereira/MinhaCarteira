using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IMovimentoBancarioRepositorio : IRepositorio<MovimentoBancario, Guid>
{
    Task<TransferenciaBancaria> CadastrarTransferencia(TransferenciaBancaria item);
    Task<int> IncluirRange(List<MovimentoBancario> itens);
}
