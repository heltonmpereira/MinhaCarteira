using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Modelo.Usuario;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class UsuarioRepositorio(IDbContext contexto) : BaseRepositorio<Usuario, Guid>(contexto), IUsuarioRepositorio
{
    protected override IQueryable<Usuario> AdicionarIncludes(IQueryable<Usuario> source, params object[] args)
    {
        return source
            .Include(i => i.Organizacao)
            .Include(i => i.Papeis)
                .ThenInclude(ti => ti.Papel);
    }

    public async Task<bool> AlterarSenha(Usuario model)
    {
        Tabela.Attach(model);
        Contexto.Entry(model).Property(p => p.PasswordHash).IsModified = true;
        Contexto.Entry(model).Property(p => p.CodigoRedefinicaoSenha).IsModified = true;
        await Contexto.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AlterarSenhaComCodigoRedefinicao(Usuario model)
    {
        Tabela.Attach(model);
        Contexto.Entry(model).Property(p => p.PasswordHash).IsModified = true;
        Contexto.Entry(model).Property(p => p.CodigoRedefinicaoSenha).IsModified = true;
        await Contexto.SaveChangesAsync();

        return true;
    }

    public async Task<Organizacao> Registrar(Organizacao organizacao)
    {
        try
        {
            var user = organizacao.Administrador;
            organizacao.Administrador = null;
            await Contexto.Organizacoes.AddAsync(organizacao).ConfigureAwait(false);

            await Contexto.SaveChangesAsync().ConfigureAwait(false);

            user.Organizacao = organizacao;
            await Contexto.Usuarios.AddAsync(user).ConfigureAwait(false);
            await Contexto.SaveChangesAsync().ConfigureAwait(false);

            organizacao.Administrador = user;

            Contexto.ChangeTracker.Clear();
            return organizacao;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<UsuarioRedefinirSenha> RedefinirSenha(Usuario model)
    {
        var codigo = $"{DateTime.Now:yyyyMMdd hhmmssffff}";
        model.CodigoRedefinicaoSenha = Convert.ToBase64String(
            Encoding.ASCII.GetBytes(codigo));

        Tabela.Attach(model);
        Contexto.Entry(model).Property(p => p.CodigoRedefinicaoSenha).IsModified = true;
        await Contexto.SaveChangesAsync();

        var retorno = new UsuarioRedefinirSenha()
        {
            Id = model.Id,
            CodigoRedefinicaoSenha = model.CodigoRedefinicaoSenha,
        };

        return retorno;
    }
}