using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using Microsoft.Extensions.Logging;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Helper;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Modelo.Extrato;
using MinhaCarteira.Servico.Helper;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class ImportarArquivoServico(
    IImportarArquivoRepositorio repositorio,
    IMovimentoBancarioServico movimentoBancarioServico,
    IRegraImportacaoServico regraImportacaoServico,
    IContaBancariaServico contaBancariaServico,
    ICategoriaServico categoriaServico,
    IPessoaServico pessoaServico,
    ICentroClassificacaoServico centroClassificacaoServico,
    ILogger<ImportarArquivoServico> logger)
    : BaseServico<ImportarArquivo, Guid, IImportarArquivoRepositorio>(repositorio), IImportarArquivoServico
{
    private const string NomeClassificacaoPadrao = "Não classificada";
    private const double LimiteSimilaridade = 0.5;

    public async Task<IRespostaPaginadaServico<MovimentoBancario>> ProcessarArquivo(FileUploadModel model)
    {
        if (model.ContaBancariaId != Guid.Empty)
        {
            var contaDb = await contaBancariaServico.ObterPorId(model.ContaBancariaId);
            model.ContaBancaria = contaDb.Dados;
        }

        var criterio = new FiltroBase { AdicionarIncludes = true, ItensPorPagina = 1 };
        criterio.AdicionarFiltroProprietario(model.ProprietarioId.ToString());

        var regras = await regraImportacaoServico.Navegar(criterio);

        var importacao = new ImportacaoExtrato(model);
        var registros = await importacao.ExtrairRegistros(
            proprietarioId: model.ProprietarioId,
            regras: regras.Dados,
            pessoaPadrao: null,
            categoriaPadrao: null,
            centroClassificacaoPadrao: null);

        var retorno = new RespostaPaginadaServico<MovimentoBancario>(registros)
        {
            BemSucedido = registros.Count > 0,
            Mensagem = "Registros processados com sucesso."
        };

        return retorno;
    }

    public async Task<IRespostaServico<bool>> ImportarMovimentos(List<MovimentoBancario> itens)
    {
        var movimentosValidos = itens
            .Where(w => w.PessoaId != Guid.Empty &&
                        w.CategoriaId != Guid.Empty &&
                        w.CentroClassificacaoId != Guid.Empty)
            .ToList();

        if (!movimentosValidos.Any())
            return new RespostaServico<bool>(false);

        var import = new ImportarArquivo()
        {
            ContaBancariaId = itens.FirstOrDefault()!.ContaBancariaId,
            ProprietarioId = itens.FirstOrDefault()!.ProprietarioId
        };
        var importDb = await Incluir(import);
        movimentosValidos.ForEach(f => f.ImportarArquivoId = importDb.Dados.Id);
        var itensDb = await movimentoBancarioServico.IncluirRange(movimentosValidos);

        var retorno = new RespostaServico<bool>(itensDb.Dados > 0);
        return retorno;

    }

    public async Task<IRespostaServico<MovimentoBancario>> ImportarNotificacao(NotificacaoModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Texto))
        {
            logger.LogWarning("Importação de notificação ignorada: texto não informado. ContaBancariaId={ContaBancariaId}", model?.ContaBancariaId);
            return new RespostaServico<MovimentoBancario>(null, "O texto da notificação é obrigatório.")
            {
                BemSucedido = false
            };
        }

        logger.LogInformation(
            "Notificação bancária recebida. ProprietarioId={ProprietarioId} ContaBancariaId={ContaBancariaId} Texto={Texto}",
            model.ProprietarioId, model.ContaBancariaId, model.Texto);

        var contaDb = await contaBancariaServico.ObterPorId(model.ContaBancariaId);
        if (contaDb.Dados == null)
        {
            logger.LogWarning(
                "Importação de notificação ignorada: conta bancária não localizada. ContaBancariaId={ContaBancariaId}",
                model.ContaBancariaId);
            return new RespostaServico<MovimentoBancario>(null, "Conta bancária não localizada.")
            {
                BemSucedido = false
            };
        }

        var tipoNotificacao = NotificacaoBancariaParser.Classificar(model.Texto);
        var valor = NotificacaoBancariaParser.ExtrairValor(model.Texto);
        var descricao = NotificacaoBancariaParser.ExtrairDescricao(model.Texto);
        var dataMovimento = NotificacaoBancariaParser.ExtrairDataHora(model.Texto);

        logger.LogInformation(
            "Notificação classificada. Tipo={TipoNotificacao} Valor={Valor} DataMovimento={DataMovimento} Descricao={Descricao}",
            tipoNotificacao, valor, dataMovimento, descricao);

        if (!NotificacaoBancariaParser.EhImportavel(tipoNotificacao))
        {
            logger.LogInformation(
                "Notificação não importada (não representa compra/pagamento efetuado). Tipo={TipoNotificacao} Valor={Valor} DataMovimento={DataMovimento} Texto={Texto}",
                tipoNotificacao, valor, dataMovimento, model.Texto);
            return new RespostaServico<MovimentoBancario>(null, $"Notificação ignorada: {tipoNotificacao}.")
            {
                BemSucedido = false
            };
        }

        if (valor <= 0)
        {
            logger.LogWarning("Notificação não importada: valor não identificado. Texto={Texto}", model.Texto);
            return new RespostaServico<MovimentoBancario>(null, "Não foi possível identificar o valor na notificação.")
            {
                BemSucedido = false
            };
        }

        var criterio = new FiltroBase { AdicionarIncludes = true, ItensPorPagina = 1 };
        criterio.AdicionarFiltroProprietario(model.ProprietarioId.ToString());
        var regras = await regraImportacaoServico.Navegar(criterio);

        var movimento = new MovimentoBancario
        {
            ProprietarioId = model.ProprietarioId,
            ContaBancariaId = model.ContaBancariaId,
            Descricao = descricao,
            TipoMovimento = NotificacaoBancariaParser.ExtrairTipo(model.Texto),
            Valor = valor,
            DataMovimento = dataMovimento,
            Competencia = dataMovimento.ToString("yyyyMM"),
            Observacao = model.Texto
        };

        // 1) Regras explícitas de importação têm precedência.
        movimento = RegraImportacaoProcessador
            .Processar([movimento], regras.Dados, null, null, null)
            .First();

        // 2) Reutiliza a classificação de um movimento similar (mesma descrição ou >= 80% de match).
        await AplicarClassificacaoPorMovimentoSimilar(movimento);

        // 3) Padrões apenas para o que permanecer sem classificação.
        var categoriaPadrao = await ObterCategoriaPadrao(model.ProprietarioId);
        var pessoaPadrao = await ObterPessoaPadrao(model.ProprietarioId);
        var centroClassificacaoPadrao = await ObterCentroClassificacaoPadrao(model.ProprietarioId);

        movimento = RegraImportacaoProcessador
            .Processar([movimento], Array.Empty<RegraImportacao>(), pessoaPadrao, categoriaPadrao, centroClassificacaoPadrao)
            .First();

        var itensDb = await movimentoBancarioServico.Incluir(movimento);

        if (itensDb.Dados != null)
        {
            logger.LogInformation(
                "Notificação importada com sucesso. MovimentoBancarioId={MovimentoBancarioId} Tipo={TipoNotificacao} Valor={Valor} DataMovimento={DataMovimento} Descricao={Descricao}",
                itensDb.Dados.Id, tipoNotificacao, valor, dataMovimento, descricao);
        }
        else
        {
            logger.LogError(
                "Falha ao importar notificação. Tipo={TipoNotificacao} Valor={Valor} DataMovimento={DataMovimento} Texto={Texto}",
                tipoNotificacao, valor, dataMovimento, model.Texto);
        }

        return new RespostaServico<MovimentoBancario>(itensDb.Dados)
        {
            BemSucedido = itensDb.Dados != null,
            Mensagem = itensDb.Dados != null
                ? "Notificação importada com sucesso."
                : "Falha ao importar a notificação."
        };
    }

    /// <summary>
    /// Reutiliza pessoa, categoria e centro de classificação de um movimento bancário já existente
    /// cuja descrição seja a mesma ou tenha ao menos 80% de similaridade com a do movimento informado.
    /// Só sobrescreve campos que ainda estejam sem classificação (após regras de importação).
    /// </summary>
    private async Task AplicarClassificacaoPorMovimentoSimilar(MovimentoBancario movimento)
    {
        if (movimento.PessoaId != Guid.Empty &&
            movimento.CategoriaId != Guid.Empty &&
            movimento.CentroClassificacaoId != Guid.Empty)
            return;

        var similar = await ObterMovimentoSimilar(movimento.ProprietarioId, movimento.Descricao);
        if (similar == null)
            return;

        var aplicado = false;

        if (movimento.PessoaId == Guid.Empty && similar.PessoaId != Guid.Empty)
        {
            movimento.PessoaId = similar.PessoaId;
            movimento.Pessoa = similar.Pessoa;
            aplicado = true;
        }

        if (movimento.CategoriaId == Guid.Empty && similar.CategoriaId != Guid.Empty)
        {
            movimento.CategoriaId = similar.CategoriaId;
            movimento.Categoria = similar.Categoria;
            aplicado = true;
        }

        if (movimento.CentroClassificacaoId == Guid.Empty && similar.CentroClassificacaoId != Guid.Empty)
        {
            movimento.CentroClassificacaoId = similar.CentroClassificacaoId;
            movimento.CentroClassificacao = similar.CentroClassificacao;
            aplicado = true;
        }

        if (aplicado)
        {
            logger.LogInformation(
                "Classificação reutilizada de movimento similar. MovimentoSimilarId={MovimentoSimilarId} DescricaoSimilar={DescricaoSimilar} PessoaId={PessoaId} CategoriaId={CategoriaId} CentroClassificacaoId={CentroClassificacaoId}",
                similar.Id, similar.Descricao, movimento.PessoaId, movimento.CategoriaId, movimento.CentroClassificacaoId);
        }
    }

    /// <summary>
    /// Localiza o movimento bancário do proprietário com descrição igual ou mais próxima (>= 80%)
    /// da descrição informada.
    /// </summary>
    private async Task<MovimentoBancario> ObterMovimentoSimilar(Guid proprietarioId, string descricao)
    {
        var termoBusca = ObterTermoBusca(descricao);
        if (string.IsNullOrWhiteSpace(termoBusca))
            return null;

        var criterio = new FiltroBase { AdicionarIncludes = true, ItensPorPagina = 50 };
        criterio.AdicionarFiltro("Descricao", new FiltroOpcao
        {
            NomePropriedade = "Descricao",
            Operador = TipoOperadorBusca.Contem,
            Valor = termoBusca
        });
        criterio.AdicionarFiltroProprietario(proprietarioId.ToString());

        var resposta = await movimentoBancarioServico.Navegar(criterio);
        var candidatos = resposta.Dados;
        if (candidatos == null || !candidatos.Any())
            return null;

        MovimentoBancario maisSimilar = null;
        var maiorSimilaridade = 0d;

        foreach (var candidato in candidatos)
        {
            var similaridade = CalcularSimilaridade(descricao, candidato.Descricao);
            if (similaridade > maiorSimilaridade)
            {
                maiorSimilaridade = similaridade;
                maisSimilar = candidato;
            }
        }

        return maiorSimilaridade >= LimiteSimilaridade
            ? maisSimilar
            : null;
    }

    /// <summary>
    /// Seleciona o token mais longo e significativo da descrição para restringir a busca no banco.
    /// </summary>
    private static string ObterTermoBusca(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            return null;

        return descricao
            .Split([' ', ',', ';', '.', '-', '/', '*'], StringSplitOptions.RemoveEmptyEntries)
            .Where(palavra => palavra.Length >= 3)
            .OrderByDescending(palavra => palavra.Length)
            .FirstOrDefault();
    }

    /// <summary>
    /// Similaridade (0 a 1) entre duas descrições, baseada na distância de Levenshtein.
    /// </summary>
    private static double CalcularSimilaridade(string textoA, string textoB)
    {
        if (string.IsNullOrWhiteSpace(textoA) || string.IsNullOrWhiteSpace(textoB))
            return 0;

        var a = NormalizarDescricao(textoA);
        var b = NormalizarDescricao(textoB);

        if (a.Length == 0 || b.Length == 0)
            return 0;

        if (a == b)
            return 1;

        var distancia = DistanciaLevenshtein(a, b);
        return 1d - ((double)distancia / Math.Max(a.Length, b.Length));
    }

    private static string NormalizarDescricao(string texto) =>
        string.Join(' ', texto.Trim().ToUpperInvariant()
            .Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries));

    private static int DistanciaLevenshtein(string a, string b)
    {
        var anterior = new int[b.Length + 1];
        var atual = new int[b.Length + 1];

        for (var j = 0; j <= b.Length; j++)
            anterior[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            atual[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var custo = a[i - 1] == b[j - 1] ? 0 : 1;
                atual[j] = Math.Min(
                    Math.Min(atual[j - 1] + 1, anterior[j] + 1),
                    anterior[j - 1] + custo);
            }

            (anterior, atual) = (atual, anterior);
        }

        return anterior[b.Length];
    }

    private static ICriterio CriarCriterioClassificacaoPadrao(Guid proprietarioId)
    {
        var criterio = new FiltroBase { ItensPorPagina = 1 };
        criterio.AdicionarFiltro("Nome", new FiltroOpcao
        {
            NomePropriedade = "Nome",
            Operador = TipoOperadorBusca.Igual,
            Valor = NomeClassificacaoPadrao
        });
        criterio.AdicionarFiltroProprietario(proprietarioId.ToString());

        return criterio;
    }

    private async Task<Categoria> ObterCategoriaPadrao(Guid proprietarioId)
    {
        var resposta = await categoriaServico.Navegar(CriarCriterioClassificacaoPadrao(proprietarioId));
        var categoria = resposta.Dados?.FirstOrDefault();
        if (categoria != null)
            return categoria;

        var incluir = await categoriaServico.Incluir(new Categoria
        {
            Nome = NomeClassificacaoPadrao,
            Caminho = NomeClassificacaoPadrao,
            ProprietarioId = proprietarioId
        });

        return incluir.Dados;
    }

    private async Task<Pessoa> ObterPessoaPadrao(Guid proprietarioId)
    {
        var resposta = await pessoaServico.Navegar(CriarCriterioClassificacaoPadrao(proprietarioId));
        var pessoa = resposta.Dados?.FirstOrDefault();
        if (pessoa != null)
            return pessoa;

        var incluir = await pessoaServico.Incluir(new Pessoa
        {
            Nome = NomeClassificacaoPadrao,
            ProprietarioId = proprietarioId
        });

        return incluir.Dados;
    }

    private async Task<CentroClassificacao> ObterCentroClassificacaoPadrao(Guid proprietarioId)
    {
        var resposta = await centroClassificacaoServico.Navegar(CriarCriterioClassificacaoPadrao(proprietarioId));
        var centro = resposta.Dados?.FirstOrDefault();
        if (centro != null)
            return centro;

        var incluir = await centroClassificacaoServico.Incluir(new CentroClassificacao
        {
            Nome = NomeClassificacaoPadrao,
            EhReceita = true,
            EhDespesa = true,
            ProprietarioId = proprietarioId
        });

        return incluir.Dados;
    }
}
