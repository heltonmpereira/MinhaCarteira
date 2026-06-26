
using System;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class LogServico(ILogRepositorio repositorio)
    : BaseServico<Log, Guid, ILogRepositorio>(repositorio), ILogServico
{
}
