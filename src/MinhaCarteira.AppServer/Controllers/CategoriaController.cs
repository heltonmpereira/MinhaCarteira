using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

public class CategoriaController(ICategoriaServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<Categoria, Guid, ICategoriaServico, ICategoriaRepositorio>(servico, httpContextAccessor)
{
    protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.FiltrarPorUsuarioLogadoOuSemUsuarioEspecificado;
}
