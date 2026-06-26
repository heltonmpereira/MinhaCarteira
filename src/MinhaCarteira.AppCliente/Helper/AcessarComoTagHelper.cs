using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MinhaCarteira.AppCliente.Helper;

[HtmlTargetElement(Attributes = "asp-acessando-como")]
public class AcessarComoTagHelper : BaseTagHelper
{
    private readonly string _acessadoVia;

    public AcessarComoTagHelper(IActionContextAccessor accessor) : base(accessor)
    {
        if (accessor.ActionContext == null) return;
        var user = accessor.ActionContext.HttpContext.User;
        _acessadoVia = user.FindFirst("AcessadoVia")?.Value;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        if (!string.IsNullOrWhiteSpace(_acessadoVia))
            AdicionarClassDesabilitar(output, "asp-acessando-como");
    }
}