using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class PapelServico(IPapelRepositorio repositorio)
    : BaseServico<Papel, Guid, IPapelRepositorio>(repositorio), IPapelServico
{
    public async Task<IRespostaServico<Papel>> AtualizarUsuarios(Guid id, Guid[] idsUsuario)
    {
        var retorno = await Repositorio.AtualizarUsuarios(id, idsUsuario);

        return await Task.FromResult<IRespostaServico<Papel>>(new RespostaServico<Papel>(retorno));
    }
}