using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MinhaCarteira.AppCliente.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Helper;

[HtmlTargetElement("asp-cabecalho")]
public class FormatarCabecalhoColunaTagHelper(IHtmlGenerator htmlGenerator) : TagHelper
{
    private void CarregarPropriedades()
    {
        Campo ??= AspFor?.Name;
        Titulo ??= AspFor?.Metadata?.DisplayName;

        if (string.IsNullOrEmpty(Ordenacao) &&
            ViewContext != null &&
            ViewContext.ViewData?.Model is IListaBaseViewModel lista)
        {
            Ordenacao = lista.Filtro?.Ordenacao ?? string.Empty;
        }
    }
    private async Task<IHtmlContentContainer> GerarLink()
    {
        var itens = Ordenacao?.Split(',').Select(s => s.Trim()).ToList();
        var classe = itens.Intersect(Campo.Split(";")).Any()
            ? "fa-sort-up"
            : itens.Intersect(Campo.Split(";").Select(s => s + " desc")).Any()
                ? "fa-sort-down"
                : "fa-sort";

        var link = $@"<i class=""fas {classe}""></i>&nbsp;<span>{Titulo ?? Campo}</span>";

        var anchorTagHelper = new AnchorTagHelper(htmlGenerator)
        {
            Action = string.Empty,
            ViewContext = ViewContext
        };

        var anchorOutput = new TagHelperOutput(
            "a",
            [],
            (useCachedResult, encoder) =>
                Task.Factory.StartNew<TagHelperContent>(() =>
                    new DefaultTagHelperContent()));

        anchorOutput.Content.AppendHtml(link);
        anchorOutput.Attributes.SetAttribute("class", "d-inline-block text-nowrap");
        anchorOutput.Attributes.SetAttribute("id", Campo);

        var anchorContext = new TagHelperContext(
            new TagHelperAttributeList(new[]
            {
                new TagHelperAttribute("asp-action", new HtmlString("Home"))
            }),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());

        await anchorTagHelper.ProcessAsync(anchorContext, anchorOutput);
        return anchorOutput;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        CarregarPropriedades();
        output.TagName = string.Empty;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.SetHtmlContent(await GerarLink());
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { set; get; }
    public ModelExpression AspFor { get; set; }
    public string Ordenacao { get; set; }
    public string Campo { get; set; }
    public string Titulo { get; set; }
}