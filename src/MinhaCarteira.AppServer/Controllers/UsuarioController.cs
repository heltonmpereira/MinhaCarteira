using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Servico.Helper;

namespace MinhaCarteira.AppServer.Controllers;

public class UsuarioController(IUsuarioServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<Usuario, Guid, IUsuarioServico, IUsuarioRepositorio>(servico, httpContextAccessor)
{
    protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.NaoFiltrarUsuario;

    public override Task<IActionResult> Incluir(Usuario item)
    {
        item.PasswordHash = item.PasswordHash.CriptografarTexto();
        return base.Incluir(item);
    }
}