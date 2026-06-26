using Microsoft.Extensions.Configuration;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Helper;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Repositorio
{
    public class ArquivoRepositorio(IDbContext contexto, IConfiguration _config) :
        BaseRepositorio<Arquivo, Guid>(contexto), IArquivoRepositorio
    {
        public override async Task<Arquivo> ObterPorId(Guid id, bool adicionarIncludes = true, params object[] args)
        {
            var itemDb = await base.ObterPorId(id, adicionarIncludes, args);

            if (itemDb != null)
                itemDb.Conteudo = await itemDb.LerBase64CifradaAsync(_config);

            return itemDb;
        }
    }
}
