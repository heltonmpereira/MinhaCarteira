using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IRegistroAuditoriaServico
{
    Task RegistrarAuditoriaAsync(
        string acao,
        string entidade,
        string entidadeId,
        string dadosAntigos,
        string dadosNovos,
        string ipUsuario,
        string userAgent,
        string rotina,
        Guid? usuarioId,
        Guid? organizacaoId);
}
