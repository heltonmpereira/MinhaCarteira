using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

public class InstituicaoFinanceiraController(
    IInstituicaoFinanceiraServico servico,
    IHttpContextAccessor httpContextAccessor)
    : BaseApiController<InstituicaoFinanceira, Guid, IInstituicaoFinanceiraServico, IInstituicaoFinanceiraRepositorio>(
        servico, httpContextAccessor)
{
    protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.NaoFiltrarUsuario;
}
