using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using System;

namespace MinhaCarteira.AppServer.Controllers
{
    public class ArquivoController(IArquivoServico servico, IHttpContextAccessor httpContextAccessor)
        : BaseApiController<Arquivo, Guid, IArquivoServico, IArquivoRepositorio>(servico, httpContextAccessor)
    {
        protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.FiltrarPorUsuarioLogadoOuSemUsuarioEspecificado;
    }
}
