using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IPapelRepositorio : IRepositorio<Papel, Guid>
{
    Task<Papel> AtualizarUsuarios(Guid id, Guid[] idsUsuario);
}