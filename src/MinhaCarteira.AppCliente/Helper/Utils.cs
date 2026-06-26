using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Helper;

public static class Utils
{
    public static async Task<List<SelectListItem>> FiltrosModulo()
    {
        var opcoes = new List<SelectListItem>([new SelectListItem("Id", "Id")]);

        return await Task.FromResult(opcoes);
    }

    public static void CriarCookie(
        this HttpResponse response,
        string key,
        string valor)
    {
        var cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMonths(12)
        };

        response.Cookies.Append(
            key,
            valor,
            cookieOptions);
    }

    public static string Compress(this string s)
    {
        var bytes = Encoding.Unicode.GetBytes(s);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        {
            msi.CopyTo(gs);
        }
        return Convert.ToBase64String(mso.ToArray());
    }

    public static string Decompress(this string s)
    {
        var bytes = Convert.FromBase64String(s);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        {
            gs.CopyTo(mso);
        }
        return Encoding.Unicode.GetString(mso.ToArray());
    }

    public static string GetDescription<T>(this T enumValue)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        var description = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        return description;
    }
	
    public static ICriterio AdicionarFiltroProprietario(
        this ICriterio criterio,
        string idUsuarioLogado,
        string nomePropriedade = "ProprietarioId")
    {
        var filtroUsuarioLogado = new FiltroOpcao(
            nomePropriedade,
            TipoOperadorBusca.Igual,
            idUsuarioLogado,
            false);

        criterio ??= new FiltroBase();
        var grupo = new GrupoFiltro("Sistema", new[] { filtroUsuarioLogado });
        criterio.AdicionarGrupo(grupo);

        return criterio;
    }

    public static ICriterio AdicionarFiltroProprietarioOpcional(
        this ICriterio criterio,
        string idUsuarioLogado,
        string nomePropriedade = "ProprietarioId")
    {
        var filtroUsuarioLogado = new FiltroOpcao(
            nomePropriedade,
            TipoOperadorBusca.Igual,
            idUsuarioLogado,
            false,
            TipoOperadorLogico.Or);

        var filtroUsuarioNulo = new FiltroOpcao(
            nomePropriedade,
            TipoOperadorBusca.Igual,
            null,
            false,
            TipoOperadorLogico.Or);

        var grp = new GrupoFiltro("Proprietario")
        {
            RelacaoEntreFiltros = TipoOperadorLogico.Or
        };
        grp.AdicionarFiltro(filtroUsuarioNulo);
        grp.AdicionarFiltro(filtroUsuarioLogado);

        criterio ??= new FiltroBase();
        criterio.AdicionarGrupo(grp);

        return criterio;
    }

    public static ICriterio OrganizarIdFiltros(this ICriterio criterio)
    {
        var idxFiltro = 1;
        foreach (var grupoFiltroFiltro in criterio.GruposFiltro.SelectMany(grupoFiltro => grupoFiltro.Filtros.Where(w => w != null)))
            grupoFiltroFiltro.Id = idxFiltro++;

        return criterio;
    }
}