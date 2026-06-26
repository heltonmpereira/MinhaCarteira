using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Relatorio;
using MinhaCarteira.Modelo.Repositorio;
using MinhaCarteira.Servico.Relatorio;
using MinhaCarteira.Servico.Servico;

namespace MinhaCarteira.AppServer.Helper;

public static class ServidorMiddleware
{
    public static IServiceCollection AdicionarDados(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IDbContext, MinhaCarteiraContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        services.AddDbContext<RelatorioContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ILoginServico, LoginServico>();

        services.AddScoped<IPapelRepositorio, PapelRepositorio>();
        services.AddScoped<IPapelServico, PapelServico>();

        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IUsuarioServico, UsuarioServico>();

        services.AddScoped<IInstituicaoFinanceiraRepositorio, InstituicaoFinanceiraRepositorio>();
        services.AddScoped<IInstituicaoFinanceiraServico, InstituicaoFinanceiraServico>();

        services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        services.AddScoped<ICategoriaServico, CategoriaServico>();

        services.AddScoped<ICentroClassificacaoRepositorio, CentroClassificacaoRepositorio>();
        services.AddScoped<ICentroClassificacaoServico, CentroClassificacaoServico>();

        services.AddScoped<IPessoaRepositorio, PessoaRepositorio>();
        services.AddScoped<IPessoaServico, PessoaServico>();

        services.AddScoped<IContaBancariaRepositorio, ContaBancariaRepositorio>();
        services.AddScoped<IContaBancariaServico, ContaBancariaServico>();

        services.AddScoped<IMovimentoBancarioRepositorio, MovimentoBancarioRepositorio>();
        services.AddScoped<IMovimentoBancarioServico, MovimentoBancarioServico>();

        services.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();
        services.AddScoped<IAgendamentoServico, AgendamentoServico>();

        services.AddScoped<IAgendamentoParcelaRepositorio, AgendamentoParcelaRepositorio>();

        services.AddScoped<IRegraImportacaoRepositorio, RegraImportacaoRepositorio>();
        services.AddScoped<IRegraImportacaoServico, RegraImportacaoServico>();

        services.AddScoped<IImportarArquivoRepositorio, ImportarArquivoRepositorio>();
        services.AddScoped<IImportarArquivoServico, ImportarArquivoServico>();

        services.AddScoped<IDashboardMonitorRepositorio, DashboardMonitorRepositorio>();
        services.AddScoped<IDashboardMonitorServico, DashboardMonitorServico>();

        services.AddScoped<IConciliacaoBancariaRepositorio, ConciliacaoBancariaRepositorio>();
        services.AddScoped<IConciliacaoBancariaServico, ConciliacaoBancariaServico>();

        services.AddScoped<IArquivoRepositorio, ArquivoRepositorio>();
        services.AddScoped<IArquivoServico, ArquivoServico>();

        services.AddScoped<IAuditoriaRepositorio, AuditoriaRepositorio>();
        services.AddScoped<IAuditoriaServico, AuditoriaServico>();
        services.AddScoped<IRegistroAuditoriaServico, RegistroAuditoriaServico>();
        
        services.AddScoped<ILogRepositorio, LogRepositorio>();
        services.AddScoped<ILogServico, LogServico>();

        services.AddScoped<RelatorioServico>();
        services.AddScoped<RelatorioRepositorio>();

        services.AddScoped<IHomeServico, HomeServico>();

        return services;
    }
}
