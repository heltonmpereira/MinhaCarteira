using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IPapelServico : IServico<Papel, Guid, IPapelRepositorio>
{
    Task<IRespostaServico<Papel>> AtualizarUsuarios(Guid id, Guid[] idsUsuario);
}