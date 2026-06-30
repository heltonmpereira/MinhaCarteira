using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MinhaCarteira.AppCliente.Helper;

public abstract class BaseTagHelper(IHttpContextAccessor accessor) : TagHelper
{
    public ClaimsPrincipal User { get; private set; } = accessor.HttpContext?.User;

    protected static void AdicionarClassDesabilitar(TagHelperOutput output, string targetElement)
    {
        var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
        if (classAttr == null)
        {
            classAttr = new TagHelperAttribute("class", "disabled");
            output.Attributes.Add(classAttr);
        }
        else if (classAttr.Value == null || classAttr.Value.ToString()!.IndexOf("disabled", StringComparison.Ordinal) < 0)
        {
            output.Attributes.SetAttribute("class", classAttr.Value == null
                ? "disabled"
                : classAttr.Value.ToString() + " disabled");
        }

        output.Attributes.RemoveAll(targetElement);
    }
}