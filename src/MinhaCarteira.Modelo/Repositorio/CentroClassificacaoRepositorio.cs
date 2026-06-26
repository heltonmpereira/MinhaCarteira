using System;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class CentroClassificacaoRepositorio(IDbContext contexto)
    : BaseRepositorio<CentroClassificacao, Guid>(contexto), ICentroClassificacaoRepositorio;
