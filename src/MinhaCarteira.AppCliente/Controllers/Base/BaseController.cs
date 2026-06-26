using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinhaCarteira.AppCliente.Controllers.Interface;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Controllers.Base;

[BreadcrumbActionFilter]
public abstract class BaseController<T, TPK, TSERVICO> : PadraoController, IControllerPaginadoServico<TSERVICO>
    where T : BaseViewModel, IEntidade<TPK>
{
    protected virtual bool ExibirRegistrosDeletados { get; set; } = false;
    protected IHttpContextAccessor _httpContextAcessor;
    protected Guid IdUsuarioLogado { get; }
    protected virtual string OrdenacaoPadrao { get; set; }

    public TSERVICO Servico { get; }

    private BaseController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAcessor = httpContextAccessor;
        var idUsuarioLogado = httpContextAccessor.HttpContext?
            .User
            .FindFirst("UsuarioId")?
            .Value;

        if (!string.IsNullOrWhiteSpace(idUsuarioLogado))
            IdUsuarioLogado = Guid.Parse(idUsuarioLogado);
    }
    protected BaseController(TSERVICO servico, IHttpContextAccessor httpContextAccessor) : this(httpContextAccessor)
    {
        Servico = servico;
    }
    protected virtual ICriterio ObterFiltroPadrao(int page = 1)
    {
        return null;
    }
    public virtual List<SelectListItem> ObterColunasFiltro(Type tipo, string prefixo = null, int nivel = 0)
    {
        var propsVm = typeof(BaseViewModel).GetProperties()
            .Select(sel => sel.Name)
            .ToList();

        var propsObj = tipo.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(w => !propsVm.Contains(w.Name))
            .ToList();

        var colunas = new List<SelectListItem>();
        foreach (var pInfo in propsObj)
            if (pInfo.PropertyType is { IsClass: true, IsPrimitive: false } && nivel < 2)
            {
                var assemblyName = pInfo.PropertyType.Assembly.GetName().Name;
                if (assemblyName != null && assemblyName.StartsWith("MinhaCarteira."))
                    colunas.AddRange(ObterColunasFiltro(pInfo.PropertyType, $"{prefixo}{pInfo.Name}.", nivel + 1));
                else colunas.Add(new SelectListItem($"{prefixo}{pInfo.Name}", $"{prefixo}{pInfo.Name}"));
            }
            else colunas.Add(new SelectListItem($"{prefixo}{pInfo.Name}", $"{prefixo}{pInfo.Name}"));

        return colunas;
    }

    public virtual async Task<IActionResult> Index(int page = 1)
    {
        Response.Cookies.Delete(Constante.NOME_COOKIE_FILTRO);
        var criterio = ObterFiltroPadrao(page) ?? new FiltroBase
        {
            Pagina = page,
            Ordenacao = OrdenacaoPadrao
        };

        return await this.ChamarServicoPaginado<T, TPK, TSERVICO>(criterio, ExibirRegistrosDeletados);
    }
    [HttpPost]
    public virtual ActionResult IndexFiltrado(CriterioViewModel model)
    {
        var modelJson = JsonConvert.SerializeObject(model);
        Response.CriarCookie(
            Constante.NOME_COOKIE_FILTRO,
            modelJson.Compress());

        return RedirectToAction(nameof(IndexPaginado));
    }
    [HttpGet]
    public virtual async Task<IActionResult> IndexPaginado()
    {
        var criterioJsonComprimido = _httpContextAcessor.HttpContext.Request
            .Cookies[Constante.NOME_COOKIE_FILTRO];
        var criterioJson = string.IsNullOrWhiteSpace(criterioJsonComprimido)
            ? string.Empty
            : criterioJsonComprimido.ToString().Decompress();
        var model = JsonConvert.DeserializeObject<CriterioViewModel>(criterioJson) ?? new CriterioViewModel();

        var criterio = new FiltroBase()
        {
            Pagina = model.PaginaAtual,
            ItensPorPagina = model?.ItensPorPagina ?? 20,
            Ordenacao = model.Ordenacao
        };
        criterio.AdicionarGrupo(model.GruposFiltro);

        if (model.FiltroAtual != null && !model.FiltroAtual.NomePropriedade.StartsWith("selecione", StringComparison.CurrentCultureIgnoreCase))
            criterio.AdicionarFiltro("Tela", model.FiltroAtual);

        ModelState.Clear();
        return await this.ChamarServicoPaginado<T, TPK, TSERVICO>(criterio, ExibirRegistrosDeletados);
    }

    public virtual async Task<IActionResult> Detalhes(TPK id)
    {
        return await this.ChamarServicoView<T, TPK, TSERVICO>(id, "ObterPorId");
    }

    protected virtual async Task<T> ObterPorId(TPK id)
    {
        var resposta = await Servico
            .ExecutarServico<RespostaServico<T>, T, TPK>([id])
            .ConfigureAwait(false);

        var item = resposta.Dados;

        return item;
    }
    public virtual async Task<IActionResult> Alterar(TPK id)
    {
        var retorno = await this.ChamarServicoView<T, TPK, TSERVICO>(id, "ObterPorId");
        return retorno;
    }
    [HttpPost]
    public virtual async Task<IActionResult> Alterar(T item)
    {
        if (!ModelState.IsValid)
        {
            await InicializarObjeto(item);
            return View(item);
        }
        item = ExecutarAntesSalvar(item);
        return await this.ChamarServicoProsseguirIndex<T, TPK, TSERVICO>(item);
    }

    public virtual Task InicializarObjeto(object instancia)
    {
        _ = instancia ?? Activator.CreateInstance<T>();
        return Task.CompletedTask;
    }
    protected virtual T ExecutarAntesSalvar(T item)
    {
        return item;
    }
    public virtual async Task<IActionResult> Incluir()
    {
        var model = Activator.CreateInstance<T>();
        await InicializarObjeto(model);
        return View(await Task.FromResult(model));
    }
    [HttpPost]
    public virtual async Task<IActionResult> Incluir(T item)
    {
        if (!ModelState.IsValid)
        {
            await InicializarObjeto(item);
            return View(item);
        }

        item = ExecutarAntesSalvar(item);
        return await this.ChamarServicoProsseguirIndex<T, TPK, TSERVICO>(item);
    }

    public virtual async Task<IActionResult> Deletar(TPK id)
    {
        return await this.ChamarServicoView<T, TPK, TSERVICO>(id, "ObterPorId");
    }
    [HttpPost]
    public virtual async Task<IActionResult> Deletar(T item)
    {
        return await this.ChamarServicoProsseguirIndex<int, T, TPK, TSERVICO>(item.Id);
    }

    [HttpGet]
    public virtual async Task<IActionResult> Clonar(TPK id)
    {
        ModelState.Clear();
        var model = await ObterPorId(id);
        model.Id = default;
        await InicializarObjeto(model);
        return View(nameof(Incluir), await Task.FromResult(model));
    }

    protected virtual string COLUNA_NOME_OBTER_TODOS { get; set; } = "Nome";
    protected virtual string COLUNA_ICONE_OBTER_TODOS { get; set; } = "Icone";
    protected virtual string COLUNA_ICONE_MIME_OBTER_TODOS { get; set; } = "MimeType";
    protected virtual FiltroBase ObterFiltroPadraoProcurarTodos(string prefix)
    {
        var criterio = new FiltroBase
        {
            AdicionarIncludes = true,
            ItensPorPagina = 10
        };
        criterio.AdicionarFiltro("Tela", new FiltroOpcao()
        {
            NomePropriedade = COLUNA_NOME_OBTER_TODOS,
            Operador = TipoOperadorBusca.Contem,
            Valor = prefix
        });

        return criterio;
    }
    [HttpGet]
    public virtual async Task<JsonResult> ObterTodos(string prefix)
    {
        var criterio = ObterFiltroPadraoProcurarTodos(prefix);
        var view = await this.ChamarServicoPaginado<T, TPK, TSERVICO>(criterio, false);
        if (view is not ViewResult result)
            return Json(new
            {
                icone =
                    "data:image/png;base64,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",
                label = "Falha ao procurar registros.",
                val = Guid.Empty
            });
        var retornoApi = result.Model as ListaBaseViewModel<T>;
        var items = retornoApi?.Itens
            .Select(s => new
            {
                icone = s.GetValue(COLUNA_ICONE_OBTER_TODOS) == null
                    ? ""
                    : $"data:{s.GetValue(COLUNA_ICONE_MIME_OBTER_TODOS)};base64,{s.GetValue(COLUNA_ICONE_OBTER_TODOS)}",
                label = s.GetValue(COLUNA_NOME_OBTER_TODOS).ToString(),
                val = s.Id,
                meta = s.ToJson()
            })
            .OrderBy(s => s.label)
            .ToList();

        if (items == null || !items.Any())
            items?.Add(new
            {
                icone = "data:image/png;base64,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",
                label = "Nenhum registro localizado.",
                val = default(TPK),
                meta = default(T).ToJson()
            });

        return Json(items);

    }
}