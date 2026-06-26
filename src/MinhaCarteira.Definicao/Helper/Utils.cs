using Dhani.Utilitarios.Filtro;
using Microsoft.Extensions.Configuration;
using MinhaCarteira.Definicao.Entidade;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MinhaCarteira.Definicao.Helper;

public static class Utils
{
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

    public static async Task<string> SalvarAsync(this Arquivo arquivo, IConfiguration _config)
    {
        var basePath = _config["Repositorio:CaminhoImagens"];
        if (!string.IsNullOrEmpty(arquivo.SubPasta))
            basePath = Path.Combine(basePath, arquivo.SubPasta);

        Directory.CreateDirectory(basePath);

        var nomeArquivo = $"{arquivo.Id}{arquivo.Extensao}";
        var caminhoCompleto = Path.Combine(basePath, nomeArquivo);
        var bytes = Convert.FromBase64String(arquivo.Conteudo);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_config["Repositorio:Chave"]);
        aes.IV = Encoding.UTF8.GetBytes(_config["Repositorio:IV"]);

        using var arquivoStream = new FileStream(caminhoCompleto, FileMode.Create);
        using var cryptoStream = new CryptoStream(arquivoStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

        await cryptoStream.WriteAsync(bytes);
        await cryptoStream.FlushAsync();

        return nomeArquivo;
    }

    public static void Excluir(this Arquivo arquivo, IConfiguration _config)
    {
        var basePath = _config["Repositorio:CaminhoImagens"];
        if (!string.IsNullOrEmpty(arquivo.SubPasta))
            basePath = Path.Combine(basePath, arquivo.SubPasta);

        Directory.CreateDirectory(basePath);

        var nomeArquivo = arquivo.Id + Path.GetExtension(arquivo.Nome);
        var caminhoCompleto = Path.Combine(basePath, nomeArquivo);

        if (File.Exists(caminhoCompleto))
            File.Delete(caminhoCompleto);
    }

    public static async Task<string> LerBase64CifradaAsync(this Arquivo arquivo, IConfiguration _config)
    {
        var basePath = _config["Repositorio:CaminhoImagens"];
        if (!string.IsNullOrEmpty(arquivo.SubPasta))
            basePath = Path.Combine(basePath, arquivo.SubPasta);

        Directory.CreateDirectory(basePath);

        var nomeArquivo = $"{arquivo.Id}{arquivo.Extensao}";
        var caminhoCompleto = Path.Combine(basePath, nomeArquivo);

        if (!File.Exists(caminhoCompleto))
            return null;

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_config["Repositorio:Chave"]);
        aes.IV = Encoding.UTF8.GetBytes(_config["Repositorio:IV"]);

        using var arquivoStream = new FileStream(caminhoCompleto, FileMode.Open);
        using var cryptoStream = new CryptoStream(arquivoStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var memoryStream = new MemoryStream();

        await cryptoStream.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }
}