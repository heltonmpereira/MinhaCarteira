using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MinhaCarteira.AppCliente.Helper;

[HtmlTargetElement(Attributes = "possui-papel")]
public class PossuiPapelTagHelper(IHttpContextAccessor accessor) : BaseTagHelper(accessor)
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        var attrib = output.Attributes
            .FirstOrDefault(attribute => attribute.Name == "possui-papel");
        var possuiPapel = User.IsInRole(attrib.Value.ToString());
        if (!possuiPapel)
            AdicionarClassDesabilitar(output, "possui-papel");
    }
}