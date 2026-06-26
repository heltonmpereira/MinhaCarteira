using System;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class RegraImportacaoServico(IRegraImportacaoRepositorio repositorio)
    : BaseServico<RegraImportacao, Guid, IRegraImportacaoRepositorio>(repositorio), IRegraImportacaoServico;
