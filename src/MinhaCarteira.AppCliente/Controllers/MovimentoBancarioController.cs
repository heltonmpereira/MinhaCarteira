using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using MinhaCarteira.AppCliente.ViewModel.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class MovimentoBancarioController(
    IMovimentoBancarioRefit servico,
    IContaBancariaRefit contasRefit,
    IArquivoRefit arquivoRefit,
    IHttpContextAccessor httpContextAccessor)
    : BaseController<MovimentoBancarioViewModel, Guid, IMovimentoBancarioRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "DataMovimento desc";

    protected override MovimentoBancarioViewModel ExecutarAntesSalvar(MovimentoBancarioViewModel item)
    {
        item.ProprietarioId = IdUsuarioLogado;
        for (int i = 0; i < item.MovimentoBancarioArquivos?.Count; i++)
        {
            if (!(item.MovimentoBancarioArquivos[i].Arquivo.IconeForm?.Length > 0))
                continue;

            using var ms = new MemoryStream();
            item.MovimentoBancarioArquivos[i].Arquivo.IconeForm.CopyTo(ms);
            var fileBytes = ms.ToArray();
            item.MovimentoBancarioArquivos[i].Arquivo.ProprietarioId = IdUsuarioLogado;
            item.MovimentoBancarioArquivos[i].Arquivo.Conteudo = Convert.ToBase64String(fileBytes);
            item.MovimentoBancarioArquivos[i].Arquivo.Nome = item.MovimentoBancarioArquivos[i].Arquivo.IconeForm.FileName;
            item.MovimentoBancarioArquivos[i].Arquivo.Extensao = Path.GetExtension(item.MovimentoBancarioArquivos[i].Arquivo.IconeForm.FileName);
        }

        return item;
    }

    private async Task<IList<ContaBancariaViewModel>> ObterContas()
    {
        var filtro = new FiltroBase { Ordenacao = "Ordem, DataCadastro" };
        var json = JsonConvert.SerializeObject(filtro.OrganizarIdFiltros());
        var retornoApi = await contasRefit.Navegar(json, false).ConfigureAwait(false);

        return retornoApi.Dados;
    }
    private async Task<(StaticPagedList<MovimentoBancarioViewModel> Pagina, ICriterio Filtro)> ObterMovimentos(ContaBancariaViewModel conta, CriterioViewModel criterio)
    {
        if (conta == null)
        {
            var pagedListNull = new StaticPagedList<MovimentoBancarioViewModel>(
                [], 1, 20, 0);

            return (pagedListNull, new FiltroBase());
        }

        var grpContaBancaria = new GrupoFiltro("ContaBancaria",
        [
            new FiltroOpcao
            {
                NomePropriedade = "ContaBancariaId",
                Operador = TipoOperadorBusca.Igual,
                Valor = conta.Id,
                Visivel = false
            }
        ])
        {
            PersistirGrupo = false
        };

        var filtro = new FiltroBase(grpContaBancaria)
        {
            Ordenacao = criterio.Ordenacao,
            Pagina = criterio.PaginaAtual,
            ItensPorPagina = criterio.ItensPorPagina
        };
        filtro.GruposFiltro.AddRange(criterio.GruposFiltro);

        if (criterio.FiltroAtual?.NomePropriedade?.Equals("Selecione") == false)
        {
            if (filtro.GruposFiltro?.Any(
                a => a.Filtros.Any(
                    f => f.NomePropriedade == criterio.FiltroAtual?.NomePropriedade &&
                         f.Valor == criterio.FiltroAtual?.Valor
                    )) == false)
                filtro.AdicionarFiltro("Tela", criterio.FiltroAtual);
        }

        var json = JsonConvert.SerializeObject(filtro.OrganizarIdFiltros());
        try
        {
            var retornoApi = await Servico.Navegar(json, false).ConfigureAwait(false);

            var pagina = new StaticPagedList<MovimentoBancarioViewModel>(
                retornoApi?.Dados ?? [],
                Math.Max(retornoApi?.NumeroPagina ?? 0, 1),
                Math.Max(retornoApi?.ItensPorPagina ?? 0, 20),
                retornoApi?.TotalRegistros ?? 0);

            return (pagina, filtro);
        }
        catch (Exception e)
        {
            var retornoApi = new RespostaServico<Exception>(e, e.Message);
            ViewBag.RetornoApi = retornoApi;
            return (null, filtro);
        }
    }

    private async Task<ListaMovimentoBancarioViewModel> CarregarDados(CriterioViewModel filtroVM)
    {
        var contaId = string.IsNullOrEmpty(filtroVM.MetaDados)
            ? Guid.Empty
            : Guid.Parse(filtroVM.MetaDados);

        var contas = await ObterContas();
        var contaSelecionada = contaId == Guid.Empty
            ? contas.FirstOrDefault()
            : contas.FirstOrDefault(f => f.Id == contaId);
        var (pagina, filtro) = await ObterMovimentos(contaSelecionada, filtroVM);

        filtroVM.GruposFiltro = filtro.GruposFiltro;
        filtroVM.PaginaAtual = pagina == null ? 1 : pagina.PageNumber;
        //filtroVM.ItensPorPagina = pagina.PageSize;
        filtroVM.MetaDados = contaSelecionada?.Id.ToString();

        var model = new ListaMovimentoBancarioViewModel
        {
            Contas = contas,
            Itens = pagina,
            Filtro = filtro,
            FiltroViewModel = filtroVM,
            ContaSelecionada = contaSelecionada
        };

        return model;
    }

    [HttpGet]
    public override async Task<IActionResult> Index(int page = 1)
    {
        Response.Cookies.Delete(Constante.NOME_COOKIE_FILTRO);
        return await Index(Guid.Empty);
    }
    [HttpGet]
    [Route("[controller]/[action]/{contaId:guid}")]
    public async Task<IActionResult> Index(Guid contaId = default)
    {
        Response.Cookies.Delete(Constante.NOME_COOKIE_FILTRO);
        var filtroViewModel = new CriterioViewModel()
        {
            PaginaAtual = 1,
            ItensPorPagina = 20,
            Ordenacao = OrdenacaoPadrao,
            Colunas = ObterColunasFiltro(typeof(MovimentoBancarioViewModel)),
            MetaDados = contaId.ToString()
        };

        var model = await CarregarDados(filtroViewModel);

        return View(model);
    }
    [HttpGet]
    public override async Task<IActionResult> IndexPaginado()
    {
        var criterioJsonComprimido = _httpContextAcessor.HttpContext.Request
            .Cookies[Constante.NOME_COOKIE_FILTRO];
        var criterioJson = string.IsNullOrWhiteSpace(criterioJsonComprimido)
            ? string.Empty
            : criterioJsonComprimido.ToString().Decompress();
        var filtro = JsonConvert.DeserializeObject<CriterioViewModel>(criterioJson) ?? new CriterioViewModel();

        var model = await CarregarDados(filtro);
        filtro.Colunas = ObterColunasFiltro(typeof(MovimentoBancarioViewModel));
        filtro.FiltroAtual = null;

        ModelState.Clear();
        return View(nameof(Index), model);
    }

    private async Task<ContaBancariaViewModel> ObterContaBancaria(Guid idContaBancaria)
    {
        var retContas = await contasRefit.ObterPorId(idContaBancaria).ConfigureAwait(false);
        var contaBancaria = retContas.Dados;

        return contaBancaria;
    }

    [HttpGet]
    public async Task<IActionResult> Incluir(Guid idContaBancaria)
    {
        var contaBancaria = await ObterContaBancaria(idContaBancaria);
        var model = new MovimentoBancarioViewModel()
        {
            NomeContaBancaria = contaBancaria.Nome,
            ContaBancariaId = idContaBancaria,
            ContaBancaria = contaBancaria
        };

        return View(model);
    }

    [HttpPost]
    public override async Task<IActionResult> Incluir(MovimentoBancarioViewModel item)
    {
        var view = await base.Incluir(item);

        if (view is RedirectToActionResult)
            return RedirectToAction(nameof(Index), new { contaId = item.ContaBancariaId });

        return view;
    }

    [HttpPost]
    public override async Task<IActionResult> Alterar(MovimentoBancarioViewModel item)
    {
        var view = await base.Alterar(item);

        if (view is RedirectToActionResult)
            return RedirectToAction(nameof(Index), new { contaId = item.ContaBancariaId });

        return view;
    }

    [HttpPost]
    public override async Task<IActionResult> Deletar(MovimentoBancarioViewModel item)
    {
        var view = await base.Deletar(item);

        if (view is RedirectToActionResult)
            return RedirectToAction(nameof(Index), new { contaId = item.ContaBancariaId });

        return view;
    }

    [HttpGet]
    public async Task<IActionResult> Transferir(Guid idContaBancaria)
    {
        var contaBancaria = await ObterContaBancaria(idContaBancaria);
        var item = new MovimentoBancarioTransferenciaViewModel
        {
            ContaBancaria = contaBancaria,
            ContaBancariaId = idContaBancaria
        };

        return View(item);
    }
    [HttpPost]
    public async Task<IActionResult> Transferir(MovimentoBancarioTransferenciaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var view = await this
            .ChamarServicoProsseguirHomeIndex<bool, Guid, IMovimentoBancarioRefit>(model)
            .ConfigureAwait(false);

        return view is RedirectToActionResult
            ? RedirectToAction(nameof(Index), new { contaId = model.ContaBancariaId })
            : view;
    }

    [HttpGet]
    public async Task<IActionResult> ConciliarMovimento(Guid id)
    {
        var view = await this.ChamarServicoView<MovimentoBancarioViewModel, Guid, IMovimentoBancarioRefit>(id, nameof(IMovimentoBancarioRefit.ObterPorId));
        if (view is not ViewResult result) return view;
        if (result.Model is not MovimentoBancarioViewModel mov) return view;

        var item = new ConciliarAgendamentoViewModel(mov);

        return View(item);
    }
    [HttpPost]
    public async Task<IActionResult> ConciliarMovimento(Guid idMovimento, string idParcelas)
    {
        if (!ModelState.IsValid) return View();
        return await this.ChamarServicoProsseguirIndex<int, Guid, IMovimentoBancarioRefit>(new object[] { idMovimento, idParcelas });
    }

    [HttpGet]
    public async Task<IActionResult> BaixarArquivo(Guid arquivoId)
    {
        var retornoApi = await arquivoRefit.ObterPorId(arquivoId).ConfigureAwait(false);
        var arquivo = retornoApi.Dados;
        if (arquivo == null)
            return NotFound();

        var bytes = Convert.FromBase64String(arquivo.Conteudo);

        return File(bytes, arquivo.MimeType ?? "application/octet-stream");
    }
}
