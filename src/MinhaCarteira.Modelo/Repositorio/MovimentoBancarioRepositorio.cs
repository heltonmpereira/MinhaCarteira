using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Helper;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Repositorio;

public class MovimentoBancarioRepositorio(IDbContext contexto, IConfiguration _config)
    : BaseRepositorio<MovimentoBancario, Guid>(contexto), IMovimentoBancarioRepositorio
{
    protected override IQueryable<MovimentoBancario> AdicionarIncludes(IQueryable<MovimentoBancario> source, params object[] args)
    {
        return source
            .Include(i => i.CentroClassificacao)
            .Include(i => i.Pessoa)
            .Include(i => i.ContaBancaria)
                .ThenInclude(i => i.InstituicaoFinanceira)
                    .ThenInclude(ti => ti.Icone)
            .Include(i => i.Categoria)
                .ThenInclude(ti => ti.Icone)
            .Include(i => i.TransferenciaBancaria.Origem.ContaBancaria)
            .Include(i => i.TransferenciaBancaria.Destino.ContaBancaria)
            .Include(i => i.MovimentoBancarioArquivos)
                .ThenInclude(ti => ti.Arquivo);

    }

    private async Task ConfigurarRelacionamentos(MovimentoBancario item)
    {
        if (item.CentroClassificacao != null && item.CentroClassificacao?.Id != Guid.Empty)
        {
            Contexto.Entry(item.CentroClassificacao).State =
                EntityState.Unchanged;

            item.CentroClassificacao = null;
        }

        if (item.Pessoa != null && item.Pessoa?.Id != Guid.Empty)
        {
            Contexto.Entry(item.Pessoa).State =
                EntityState.Unchanged;

            item.Pessoa = null;
        }

        if (item.Categoria != null && item.Categoria?.Id != Guid.Empty)
        {
            Contexto.Entry(item.Categoria).State =
                EntityState.Unchanged;

            item.Categoria = null;
        }

        if (item.ContaBancaria != null && (item.ContaBancaria?.Id != Guid.Empty || item.ContaBancariaId != Guid.Empty))
        {
            Contexto.Entry(item.ContaBancaria).State =
                EntityState.Unchanged;

            item.ContaBancaria = null;
        }

        if (item.TransferenciaBancaria != null && item.TransferenciaBancaria?.Id != Guid.Empty)
        {
            Contexto.Entry(item.TransferenciaBancaria).State =
                EntityState.Unchanged;

            item.TransferenciaBancaria = null;
        }

        if (item.Proprietario != null && item.Proprietario?.Id != Guid.Empty)
        {
            Contexto.Entry(item.Proprietario).State =
                EntityState.Unchanged;

            item.Proprietario = null;
        }

        for (int i = 0; i < item.MovimentoBancarioArquivos?.Count; i++)
        {
            if (item.MovimentoBancarioArquivos[i] != null && item.MovimentoBancarioArquivos[i].Id == Guid.Empty)
            {
                await Contexto.Arquivos.AddAsync(item.MovimentoBancarioArquivos[i].Arquivo);
                await Contexto.MovimentosBancarioArquivos.AddAsync(item.MovimentoBancarioArquivos[i]);

                item.MovimentoBancarioArquivos[i].ArquivoId = item.MovimentoBancarioArquivos[i].Arquivo.Id;
                item.MovimentoBancarioArquivos[i].MovimentoBancarioId = item.MovimentoBancarioArquivos[i].MovimentoBancario.Id;

                await item.MovimentoBancarioArquivos[i].Arquivo.SalvarAsync(_config);
            }
            else if (item.MovimentoBancarioArquivos[i] != null && item.MovimentoBancarioArquivos[i].Id != Guid.Empty)
            {
                Contexto.Entry(item.MovimentoBancarioArquivos[i]).State = EntityState.Unchanged;
                Contexto.Entry(item.MovimentoBancarioArquivos[i].Arquivo).State = EntityState.Unchanged;

                if (item.MovimentoBancarioArquivos[i].Deletado)
                {
                    Contexto.Entry(item.MovimentoBancarioArquivos[i]).State = EntityState.Deleted;
                    Contexto.Entry(item.MovimentoBancarioArquivos[i].Arquivo).State = EntityState.Deleted;

                    item.MovimentoBancarioArquivos[i].Arquivo.Excluir(_config);
                }

                item.MovimentoBancarioArquivos[i].ArquivoId = item.MovimentoBancarioArquivos[i].Arquivo.Id;
                item.MovimentoBancarioArquivos[i].MovimentoBancarioId = item.MovimentoBancarioArquivos[i].MovimentoBancario.Id;
            }
        }
    }

    public override async Task<MovimentoBancario> Incluir(MovimentoBancario item)
    {
        await ConfigurarRelacionamentos(item);
        var itensDb = await base.Incluir(item).ConfigureAwait(false);

        return itensDb;
    }

    protected override async Task<MovimentoBancario> ExecutarAntesAlterar(MovimentoBancario param)
    {
        var retorno = await base.ExecutarAntesAlterar(param);
        await ConfigurarRelacionamentos(param);
        return retorno;
    }

    public override async Task<int> DeletarRange(Guid[] ids)
    {
        try
        {
            var objs = Contexto.MovimentosBancarios
                .Where(w => ids.Contains(w.Id))
                .ToList();

            foreach (var item in objs)
            {
                item.TipoMovimento = item.TipoMovimento == TipoMovimento.Debito
                    ? TipoMovimento.Credito
                    : TipoMovimento.Debito;
            }

            Tabela.RemoveRange(objs);
            var retorno = await Contexto.SaveChangesAsync().ConfigureAwait(false);
            return retorno;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<TransferenciaBancaria> CadastrarTransferencia(TransferenciaBancaria item)
    {
        try
        {
            await ConfigurarRelacionamentos(item.Origem);
            await ConfigurarRelacionamentos(item.Destino);

            _ = await Contexto.MovimentosBancarios.AddAsync(item.Origem);
            _ = await Contexto.MovimentosBancarios.AddAsync(item.Destino);

            item.MovimentoOrigemId = item.Origem.Id;
            item.MovimentoDestinoId = item.Destino.Id;

            await Contexto.TransferenciasBancarias.AddAsync(item).ConfigureAwait(false);
            await Contexto.SaveChangesAsync().ConfigureAwait(false);

            await ConfigurarRelacionamentos(item.Origem);
            await ConfigurarRelacionamentos(item.Destino);
            item.Origem.TransferenciaBancariaId = item.Id;
            item.Destino.TransferenciaBancariaId = item.Id;

            Contexto.MovimentosBancarios.Update(item.Origem);
            Contexto.MovimentosBancarios.Update(item.Destino);

            await Contexto.SaveChangesAsync().ConfigureAwait(false);

            return item;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int> IncluirRange(List<MovimentoBancario> itens)
    {
        foreach (var movimentoBancario in itens)
        {
            movimentoBancario.Pessoa = null;
            movimentoBancario.Categoria = null;
            movimentoBancario.ContaBancaria = null;
            movimentoBancario.CentroClassificacao = null;
        }
        await Contexto.MovimentosBancarios.AddRangeAsync(itens);
        var retorno = await Contexto.MySaveChangesWithoutTriggersAsync();

        return retorno;
    }
}