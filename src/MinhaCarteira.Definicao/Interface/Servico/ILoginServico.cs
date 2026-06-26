using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo.Usuario;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface ILoginServico : IServico<Usuario, Guid, IUsuarioRepositorio>
{
    Task<IRespostaServico<UsuarioToken>> Login(UsuarioLogin userInfo);
    Task<IRespostaServico<UsuarioRedefinirSenha>> RedefinirSenha(UsuarioSolicitacaoRedefinicaoSenha model);
    Task<IRespostaServico<bool>> AlterarSenhaComCodigoRedefinicao(UsuarioRedefinirSenha model);
    Task<IRespostaServico<bool>> AlterarSenha(AlterarSenha model);
    Task<IRespostaServico<UsuarioToken>> AcessarComo(Guid id);
    Task<IRespostaServico<Organizacao>> Registrar(Organizacao organizacao);
}