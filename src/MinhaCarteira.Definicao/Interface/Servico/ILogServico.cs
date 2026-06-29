
using System;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface ILogServico : IServico<Log, Guid, ILogRepositorio>
{
}
