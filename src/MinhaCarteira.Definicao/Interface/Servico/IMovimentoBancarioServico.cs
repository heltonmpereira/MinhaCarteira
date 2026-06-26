using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IMovimentoBancarioServico : IServico<MovimentoBancario, Guid, IMovimentoBancarioRepositorio>
{
    Task<IRespostaServico<int>> IncluirRange(List<MovimentoBancario> item);
    Task<IRespostaServico<bool>> Transferir(MovimentoBancarioTransferencia item);
    Task<IRespostaServico<int>> ConciliarMovimento(Guid proprietarioId, Guid idMovimento, string idParcelas);
}
