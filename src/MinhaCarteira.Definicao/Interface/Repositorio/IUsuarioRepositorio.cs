using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using MinhaCarteira.Definicao.Modelo.Usuario;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IUsuarioRepositorio : IRepositorio<Usuario, Guid>
{
    Task<UsuarioRedefinirSenha> RedefinirSenha(Usuario model);
    Task<bool> AlterarSenhaComCodigoRedefinicao(Usuario model);
    Task<bool> AlterarSenha(Usuario model);
    Task<Organizacao> Registrar(Organizacao organizacao);
}