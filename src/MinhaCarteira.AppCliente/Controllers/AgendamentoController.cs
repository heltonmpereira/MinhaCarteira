using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
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
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class AgendamentoController(
    IAgendamentoRefit servico,
    IMovimentoBancarioRefit movBancarioRefit,
    IHttpContextAccessor httpContextAccessor)
    : BaseController<AgendamentoViewModel, Guid, IAgendamentoRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } =
        "EstahPaga, EstahConciliada, DataPagamento, Data, Agendamento.Descricao";
    protected override ICriterio ObterFiltroPadrao(int page = 1)
    {
        var mesAno = GetMesAnoFromRequest();
        var mes = DateTime.ParseExact(mesAno + "01", "yyyyMMdd", null);
        var inicioMes = new DateTime(mes.Year, mes.Month, 1);
        var fimMes = new DateTime(mes.Year, mes.Month, DateTime.DaysInMonth(mes.Year, mes.Month), 23, 59, 59);

        var grupoFiltroMes = new GrupoFiltro("Padrao1",
        [
            new()
            {
                NomePropriedade =  "Data",
                Operador = TipoOperadorBusca.MaiorOuIgual,
                Valor =  inicioMes,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "Data",
                Operador = TipoOperadorBusca.MenorOuIgual,
                Valor =  fimMes,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }
        ])
        {
            RelacaoOutrosGrupos = TipoOperadorLogico.Or
        };

        if (int.Parse(DateTime.Now.ToString("yyyyMM")) <= int.Parse(mesAno))
        {
            grupoFiltroMes.AdicionarFiltro(new FiltroOpcao()
            {
                NomePropriedade = "EstahPaga",
                Operador = TipoOperadorBusca.Igual,
                Valor = false,
                RelacaoOutrosFiltros = TipoOperadorLogico.And,
                Visivel = true
            });
        }

        var grupoFiltroContasAberta = new GrupoFiltro("Padrao2",
        [
            new()
            {
                NomePropriedade =  "EstahPaga",
                Operador = TipoOperadorBusca.Igual,
                Valor =  false,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "DespesaOpcional",
                Operador = TipoOperadorBusca.Igual,
                Valor =  false,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "Data",
                Operador = TipoOperadorBusca.MaiorOuIgual,
                Valor =  inicioMes,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "Data",
                Operador = TipoOperadorBusca.MenorOuIgual,
                Valor =  fimMes,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }
        ])
        {
            RelacaoOutrosGrupos = TipoOperadorLogico.Or
        };

        var grupoFiltroContasNaoConciliadas = new GrupoFiltro("Padrao3",
        [
            new()
            {
                NomePropriedade =  "EstahPaga",
                Operador = TipoOperadorBusca.Igual,
                Valor =  true,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "EstahConciliada",
                Operador = TipoOperadorBusca.Igual,
                Valor =  true,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "DataPagamento",
                Operador = TipoOperadorBusca.MaiorOuIgual,
                Valor =  inicioMes,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }, new()
            {
                NomePropriedade =  "DataPagamento",
                Operador = TipoOperadorBusca.MenorOuIgual,
                Valor =  fimMes,
                RelacaoOutrosFiltros =  TipoOperadorLogico.And,
                Visivel =  true
            }
        ]);

        var filtro = new FiltroBase
        {
            Pagina = page,
            ItensPorPagina = 1,
            Ordenacao = OrdenacaoPadrao
        };
        filtro.AdicionarGrupo(grupoFiltroMes);
        filtro.AdicionarGrupo(grupoFiltroContasAberta);
        filtro.AdicionarGrupo(grupoFiltroContasNaoConciliadas);

        return filtro;
    }

    private string GetMesAnoFromRequest()
    {
        // Try to get from query string
        var mesAnoQuery = Request.Query["mesAno"].ToString();
        if (!string.IsNullOrEmpty(mesAnoQuery) && mesAnoQuery.Length == 6 && int.TryParse(mesAnoQuery, out _))
        {
            ViewBag.MesAnoSelecionado = mesAnoQuery;
            return mesAnoQuery;
        }

        // Default to current month
        var defaultMesAno = DateTime.Now.ToString("yyyyMM");
        ViewBag.MesAnoSelecionado = defaultMesAno;
        return defaultMesAno;
    }

    private List<(string MesAno, string NomeMes, int Ano)> ObterMeses()
    {
        var mesAnoSelecionado = ViewBag.MesAnoSelecionado as string ?? DateTime.Now.ToString("yyyyMM");
        var dataInicial = DateTime.ParseExact(mesAnoSelecionado + "01", "yyyyMMdd", null);
        var isMobile = ((BrowserViewModel)ViewBag.BrowserInfo).IsMobile;
        var data = dataInicial.AddMonths(isMobile ? -2 : -6);
        var totalItens = isMobile ? 5 : 13;

        var meses = new List<(string MesAno, string NomeMes, int Ano)>();
        for (var i = 0; i < totalItens; i++)
        {
            var mesAtual = data.AddMonths(i);
            meses.Add((mesAtual.ToString("yyyyMM"), mesAtual.ToString("MMM"), mesAtual.Year));
        }

        return meses;
    }

    private ICriterio BuildFiltro(ConciliarAgendamentoViewModel model, string campoData = "DataMovimento", string prefixo = null)
    {
        var grp = new GrupoFiltro("Tela");
        if (model.ValorInicial > 0)
            grp.AdicionarFiltro(new FiltroOpcao("Valor", TipoOperadorBusca.MaiorOuIgual, model.ValorInicial));
        if (model.ValorFinal > 0)
            grp.AdicionarFiltro(new FiltroOpcao("Valor", TipoOperadorBusca.MenorOuIgual, model.ValorFinal));
        if (model.DataInicial > DateTime.MinValue)
            grp.AdicionarFiltro(new FiltroOpcao(campoData, TipoOperadorBusca.MaiorOuIgual, model.DataInicial));
        if (model.DataFinal > DateTime.MinValue)
            grp.AdicionarFiltro(new FiltroOpcao(campoData, TipoOperadorBusca.MenorOuIgual, model.DataFinal));
        if (model.ContaBancariaId > Guid.Empty)
            grp.AdicionarFiltro(new FiltroOpcao($"{prefixo}ContaBancariaId", TipoOperadorBusca.Igual, model.ContaBancariaId));
        if (model.CategoriaId > Guid.Empty)
            grp.AdicionarFiltro(new FiltroOpcao($"{prefixo}CategoriaId", TipoOperadorBusca.Igual, model.CategoriaId));
        if (!string.IsNullOrWhiteSpace(model.Descricao))
            grp.AdicionarFiltro(new FiltroOpcao("Descricao", TipoOperadorBusca.Contem, model.Descricao));

        var filtro = new FiltroBase(grp)
        {
            ItensPorPagina = 1,
            AdicionarIncludes = true
        };

        return filtro;
    }

    [HttpPost]
    public async Task<JsonResult> ObterMovimentos(ConciliarAgendamentoViewModel model)
    {
        var filtro = BuildFiltro(model);
        var json = JsonConvert.SerializeObject(filtro.OrganizarIdFiltros());
        var resp = await movBancarioRefit.Navegar(json, false).ConfigureAwait(false);

        return Json(resp.Dados);
    }

    [HttpGet]
    public async Task<JsonResult> ObterParcelas(ConciliarAgendamentoViewModel model)
    {
        var filtro = BuildFiltro(model, "Data", "Agendamento.");
        //var json = JsonConvert.SerializeObject(filtro.OrganizarIdFiltros());
        var resp = await Servico.ContasAVencer(filtro.OrganizarIdFiltros() as FiltroBase, ExibirRegistrosDeletados).ConfigureAwait(false);

        return Json(resp.Dados);
    }
    private IActionResult FormataRetornoIndex(IActionResult action)
    {
        if (action is not ViewResult result) return action;

        var ordemFiltro = result.Model.GetValue("Filtro.Ordenacao");
        if (ordemFiltro == null || !ordemFiltro.Equals(OrdenacaoPadrao))
            return action;

        result.Model.SetValue("Filtro.Ordenacao", string.Empty);
        result.Model.SetValue("FiltroViewModel.Ordenacao", string.Empty);

        return action;
    }
    public override async Task<IActionResult> Index(int page = 1)
    {
        Response.Cookies.Delete(Constante.NOME_COOKIE_FILTRO);
        var criterio = ObterFiltroPadrao(page) ?? new FiltroBase
        {
            Pagina = page,
            Ordenacao = OrdenacaoPadrao
        };

        var view = await this.ChamarServicoPaginado<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(criterio, ExibirRegistrosDeletados, "ContasAVencer", serializarFiltro: false);
        return FormataRetornoIndex(view);
    }
    [HttpGet]
    public override async Task<IActionResult> IndexPaginado()
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
            Ordenacao = model.Ordenacao ?? OrdenacaoPadrao
        };
        criterio.AdicionarGrupo(model.GruposFiltro);

        if (model.FiltroAtual != null && !model.FiltroAtual.NomePropriedade.StartsWith("selecione", StringComparison.InvariantCultureIgnoreCase))
            criterio.AdicionarFiltro("Tela", model.FiltroAtual);

        ModelState.Clear();
        var view = await this.ChamarServicoPaginado<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(criterio, ExibirRegistrosDeletados, "ContasAVencer", serializarFiltro: false);
        return FormataRetornoIndex(view);
    }

    public async Task<IActionResult> AlterarParcela(Guid id)
    {
        return await this
            .ChamarServicoView<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(id, nameof(IAgendamentoRefit.ObterParcelaPorId))
            .ConfigureAwait(false);
    }
    [HttpPost]
    public async Task<IActionResult> AlterarParcela(AgendamentoParcelaViewModel item)
    {
        if (!ModelState.IsValid) return View(item);
        return await this
            .ChamarServicoProsseguirIndex<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(item)
            .ConfigureAwait(false); ;
    }

    public async Task<IActionResult> DeletarParcela(Guid id)
    {
        return await this.ChamarServicoView<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(id, nameof(IAgendamentoRefit.ObterParcelaPorId));
    }
    [HttpPost]
    public async Task<IActionResult> DeletarParcela(AgendamentoParcelaViewModel item)
    {
        if (!ModelState.IsValid) return View(item);
        return await this.ChamarServicoProsseguirIndex<int, Guid, IAgendamentoRefit>(item.Id);
    }

    public async Task<IActionResult> PagarParcela(Guid id)
    {
        return await this.ChamarServicoView<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(id, nameof(IAgendamentoRefit.ObterParcelaPorId));
    }
    [HttpPost]
    public async Task<IActionResult> PagarParcela(AgendamentoParcelaViewModel item)
    {
        if (!ModelState.IsValid) return View(item);
        return await this.ChamarServicoProsseguirIndex<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(item);
    }

    public async Task<IActionResult> RestaurarCondicaoParcela(Guid id)
    {
        return await this.ChamarServicoView<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(id, nameof(IAgendamentoRefit.ObterParcelaPorId));
    }
    [HttpPost]
    public async Task<IActionResult> RestaurarCondicaoParcela(AgendamentoParcelaViewModel item)
    {
        if (!ModelState.IsValid) return View(item);
        return await this.ChamarServicoProsseguirIndex<int, Guid, IAgendamentoRefit>(item.Id);
    }

    public async Task<IActionResult> ConciliarParcela(Guid id)
    {
        var view = await this.ChamarServicoView<AgendamentoParcelaViewModel, Guid, IAgendamentoRefit>(id, nameof(IAgendamentoRefit.ObterParcelaPorId));
        if (view is not ViewResult result) return view;
        if (result.Model is not AgendamentoParcelaViewModel agend) return view;

        var item = new ConciliarAgendamentoViewModel(agend);

        return View(item);
    }
    [HttpPost]
    public async Task<IActionResult> ConciliarParcela(Guid idParcela, string idMovimentos)
    {
        if (!ModelState.IsValid) return View();
        return await this.ChamarServicoProsseguirIndex<int, Guid, IAgendamentoRefit>(new object[] { idParcela, idMovimentos });
    }

    protected override string COLUNA_NOME_OBTER_TODOS { get; set; } = "Descricao";
    protected override string COLUNA_ICONE_OBTER_TODOS { get; set; } = "Categoria.Icone.Conteudo";
    protected override string COLUNA_ICONE_MIME_OBTER_TODOS { get; set; } = "Categoria.Icone.MimeType";

    [HttpPost("/Agendamento/conciliar-parcelas-a-partir-monitor")]
    public async Task<IActionResult> ConciliarParcelasAPartirDashboardMonitor(string filtro)
    {
        var criterio = JsonConvert.DeserializeObject<List<GrupoFiltro>>(filtro);
        var item = criterio
            .Select(w => w.Filtros
                .Where(flt => flt.NomePropriedade == "Data" && flt.Operador == TipoOperadorBusca.MaiorOuIgual)
                .FirstOrDefault())
            .FirstOrDefault();
        item.RenderizarFiltro();
        var mesAno = Convert.ToDateTime(item.Valor).ToString("yyyyMM");

        return await this.ChamarServicoProsseguirIndex<bool, Guid, IAgendamentoRefit>(mesAno);
    }

    [HttpGet("/Agendamento/EstenderParcelasPor5Anos/{id}")]
    public async Task<IActionResult> EstenderParcelasPor5Anos(Guid id)
    {
        try
        {
            var retornoApi = await Servico.EstenderParcelasPor5Anos(id);
            var json = Newtonsoft.Json.Linq.JObject.Parse(retornoApi.ToJson());
            json.Property("Dados")?.Remove();
            TempData.Clear();
            TempData["RetornoApi"] = json.ToString();
        }
        catch (Exception ex)
        {
            var retornoApi = new RespostaServico<Exception>(ex, ex.Message);
            var json = Newtonsoft.Json.Linq.JObject.Parse(retornoApi.ToJson());
            json.Property("Dados")?.Remove();
            TempData.Clear();
            TempData["RetornoApi"] = json.ToString();
        }

        return RedirectToAction(nameof(Index));
    }

}
