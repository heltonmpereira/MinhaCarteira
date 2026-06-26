using System;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using Microsoft.AspNetCore.Http;

namespace MinhaCarteira.AppServer.Controllers;

public class RegraImportacaoController(IRegraImportacaoServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<RegraImportacao, Guid, IRegraImportacaoServico, IRegraImportacaoRepositorio>(servico,
        httpContextAccessor);
