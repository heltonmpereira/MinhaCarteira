using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Repositorio;

namespace MinhaCarteira.Servico.Servico;

public class RegistroAuditoriaServico(IAuditoriaRepositorio auditoriaRepositorio) : IRegistroAuditoriaServico
{
    public async Task RegistrarAuditoriaAsync(
        string acao,
        string entidade,
        string entidadeId,
        string dadosAntigos,
        string dadosNovos,
        string ipUsuario,
        string userAgent,
        string rotina,
        Guid? usuarioId,
        Guid? organizacaoId)
    {
        var auditoria = new Auditoria
        {
            DataHora = DateTime.Now,
            Acao = acao,
            Entidade = entidade,
            EntidadeId = entidadeId,
            DadosAntigos = dadosAntigos,
            DadosNovos = dadosNovos,
            IpUsuario = ipUsuario,
            UserAgent = userAgent,
            Rotina = rotina,
            UsuarioId = usuarioId,
            OrganizacaoId = organizacaoId
        };

        await auditoriaRepositorio.Incluir(auditoria);
    }
}
