using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using System;

namespace MinhaCarteira.AppServer.Controllers;

public class ConciliacaoBancariaController(IConciliacaoBancariaServico servico, IHttpContextAccessor httpContextAcessor)
    : BaseApiController<ConciliacaoBancaria, Guid, IConciliacaoBancariaServico, IConciliacaoBancariaRepositorio>(
        servico, httpContextAcessor);