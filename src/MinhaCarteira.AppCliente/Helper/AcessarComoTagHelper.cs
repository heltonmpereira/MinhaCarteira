using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MinhaCarteira.AppCliente.Helper;

[HtmlTargetElement(Attributes = "asp-acessando-como")]
public class AcessarComoTagHelper : BaseTagHelper
{
    private readonly string _acessadoVia;

    public AcessarComoTagHelper(IHttpContextAccessor accessor) : base(accessor)
    {
        if (accessor.HttpContext == null) return;
        var user = accessor.HttpContext.User;
        _acessadoVia = user.FindFirst("AcessadoVia")?.Value;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        if (!string.IsNullOrWhiteSpace(_acessadoVia))
            AdicionarClassDesabilitar(output, "asp-acessando-como");
    }
}