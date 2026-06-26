using System;
using System.Net;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class ContaBancariaServico(IContaBancariaRepositorio repositorio)
    : BaseServico<ContaBancaria, Guid, IContaBancariaRepositorio>(repositorio), IContaBancariaServico
{
    public async Task<IRespostaServico<bool>> Reativar(Guid id)
    {
        var itemDb = await Repositorio.Reativar(id);
        var retorno = new RespostaServico<bool>(itemDb)
        {
            BemSucedido = itemDb,
            StatusCode = HttpStatusCode.NoContent,
            Mensagem = itemDb
                ? "Conta bancária reativada com sucesso."
                : "Falha ao reativar a conta bancária."
        };

        return retorno;
    }

    public async Task<IRespostaServico<bool>> IncrementarPrioridade(Guid id)
    {
        var itemDb = await Repositorio.AlterarOrdemContaBancaria(id, 1);
        var retorno = new RespostaServico<bool>(itemDb)
        {
            BemSucedido = itemDb,
            StatusCode = HttpStatusCode.NoContent,
            Mensagem = itemDb
                ? "Pririodade da conta bancária incrementada com sucesso."
                : "Falha ao incrementar a prioridade da conta bancária."
        };

        return retorno;
    }

    public async Task<IRespostaServico<bool>> DecrementarPrioridade(Guid id)
    {
        var itemDb = await Repositorio.AlterarOrdemContaBancaria(id, -1);
        var retorno = new RespostaServico<bool>(itemDb)
        {
            BemSucedido = itemDb,
            Mensagem = itemDb
                ? "Pririodade da conta bancária decrementada com sucesso."
                : "Falha ao decrementar a prioridade da conta bancária."
        };

        return retorno;
    }

    public async Task<IRespostaServico<bool>> AtualizarSaldos()
    {
        var itemDb = await Repositorio.AtualizarSaldos();
        var retorno = new RespostaServico<bool>(itemDb)
        {
            BemSucedido = itemDb,
            Mensagem = itemDb
                ? "Saldos bancários atualizados com sucesso."
                : "Falha ao atualizar os saldos bancários."
        };

        return retorno;
    }
}
