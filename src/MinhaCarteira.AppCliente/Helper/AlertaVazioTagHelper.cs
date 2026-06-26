using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MinhaCarteira.AppCliente.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Helper;

[HtmlTargetElement("asp-alerta-vazio")]
public class AlertaVazioTagHelper(IHtmlGenerator htmlGenerator) : TagHelper
{
    private async Task<IHtmlContentContainer> GerarLink()
    {
        var anchorTagHelper = new AnchorTagHelper(htmlGenerator)
        {
            Area = Area,
            Action = Action,
            Controller = Controller,
            RouteValues = RouteValues,
            ViewContext = ViewContext
        };

        var anchorOutput = new TagHelperOutput(
            "a",
            [],
            (useCachedResult, encoder) =>
                Task.Factory.StartNew<TagHelperContent>(() =>
                    new DefaultTagHelperContent()));

        var icone = $@"<i class=""fa {IconeBotaoCadastrar}""></i>&nbsp;";
        var spanTexto = $"<span>{TextoBotaoCadastrar ?? "Cadastrar"}</span>";
        anchorOutput.Content.AppendHtml(icone);
        anchorOutput.Content.AppendHtml(spanTexto);
        anchorOutput.Attributes.SetAttribute("class", $"{ClasseBotaoCadastrar ?? $"btn btn-lg {ViewContext.ViewBag.ClasseBotao}primary"}");

        var anchorContext = new TagHelperContext(
            new TagHelperAttributeList(new[]
            {
                new TagHelperAttribute("asp-area", new HtmlString(Area)),
                new TagHelperAttribute("asp-action", new HtmlString(Action)),
                new TagHelperAttribute("asp-controller", new HtmlString(Controller))
            }),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());

        await anchorTagHelper.ProcessAsync(anchorContext, anchorOutput);
        return anchorOutput;
    }
    private static string GetString(IHtmlContentContainer content)
    {
        using var writer = new System.IO.StringWriter();
        content.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
    private async Task<string> MontarTemplateVazioAsync()
    {
        var botao = await GerarLink();
        var template = $@"<h1 class=""display-5 fw-bold"">{Titulo}</h1>
<p class=""col-md-12 fs-4"">Você não possui nenhum registro a ser listada por aqui.</p>
<p class=""col-md-12 fs-6"">Clique no botão abaixo para cadastrar um novo item.</p>

{GetString(botao)}
";

        return await Task.FromResult(template);

    }
    private async Task<string> MontarTemplateFiltroVazioAsync()
    {
        var template = $@"<h1 class=""display-5 fw-bold"">{Titulo}</h1>
<p class=""col-md-12 fs-4"">Nenhum registro foi localizado a partir do filtro especificado.</p>
<p class=""col-md-12 fs-6"">Por gentileza, remova ou especifique o filtro especificado para verificar os registros cadastrados.</p>
";

        return await Task.FromResult(template);
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.SuppressOutput();
        output.Content.Clear();

        var attributeObjects = context.AllAttributes.ToList();
        var props = this.GetType().GetProperties().Select(p => p.Name);

        attributeObjects.RemoveAll(a => props.Contains(a.Name));
        var extraAttributeList = new List<string>();
        foreach (var attr in attributeObjects)
        {
            extraAttributeList.Add($"{attr.Name}=\"{attr.Value}\"");
        }

        var conteudo = Criterio != null && 
            Criterio.GruposFiltro != null && 
            Criterio.GruposFiltro.Any(a => !a.NomeGrupoKey.StartsWith("Padrao") && a.Filtros.Any(f => f.Visivel))
            ? await MontarTemplateFiltroVazioAsync()
            : await MontarTemplateVazioAsync();

        output.TagName = "div";
        output.Content.SetHtmlContent(conteudo);
        output.TagMode = TagMode.StartTagAndEndTag;
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { set; get; }
    public ICriterioViewModel Criterio { get; set; }
    public string Titulo { get; set; } = "Título para este módulo";
    [HtmlAttributeName("asp-action")]
    public string Action { get; set; } = "Incluir";
    [HtmlAttributeName("asp-controller")]
    public string Controller { get; set; }
    [HtmlAttributeName("asp-area")]
    public string Area { get; set; }
    [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
    public IDictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();
    public string ParametroId { get; set; }
    public string ClasseBotaoCadastrar { get; set; }
    public string IconeBotaoCadastrar { get; set; } = "fa-plus-circle";
    public string TextoBotaoCadastrar { get; set; } = "Cadastrar";
}
