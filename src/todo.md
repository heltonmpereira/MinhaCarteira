# TODO

## Swagger ativo em produção, porém protegido por usuário e senha (HTTP Basic)

Objetivo: manter o Swagger **habilitado em produção**, mas exigir **usuário e senha** antes de acessar
`/swagger` e `/swagger/v1/swagger.json`. Em desenvolvimento continua livre.

Contexto: isto **substitui** o passo 1 do item D5 (que apenas escondia o Swagger em produção).

### Passos

- [ ] **1. `using`s no topo de `MinhaCarteira.AppServer/Startup.cs`**
  ```csharp
  using System;
  using System.Security.Cryptography;
  using System.Text;
  ```

- [ ] **2. Em `Startup.Configure`, trocar o bloco do Swagger por este:**
  ```csharp
  var swaggerUsuario = Configuration["Swagger:Usuario"];
  var swaggerSenha = Configuration["Swagger:Senha"];
  var swaggerProtegido = !string.IsNullOrWhiteSpace(swaggerUsuario);

  // Em Development: Swagger livre. Em Production: só liga se houver credenciais.
  if (!env.IsProduction() || swaggerProtegido)
  {
      if (swaggerProtegido)
      {
          app.Use(async (context, next) =>
          {
              if (!context.Request.Path.StartsWithSegments("/swagger"))
              {
                  await next();
                  return;
              }

              var cabecalho = context.Request.Headers.Authorization.ToString();

              if (cabecalho.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
              {
                  try
                  {
                      var decodificado = Encoding.UTF8.GetString(
                          Convert.FromBase64String(cabecalho["Basic ".Length..]));
                      var partes = decodificado.Split(':', 2);

                      if (partes.Length == 2 &&
                          CryptographicOperations.FixedTimeEquals(
                              Encoding.UTF8.GetBytes(partes[0]), Encoding.UTF8.GetBytes(swaggerUsuario)) &&
                          CryptographicOperations.FixedTimeEquals(
                              Encoding.UTF8.GetBytes(partes[1]), Encoding.UTF8.GetBytes(swaggerSenha ?? string.Empty)))
                      {
                          await next();
                          return;
                      }
                  }
                  catch (FormatException)
                  {
                      // header Base64 inválido/ausente → cai no 401 abaixo
                  }
              }

              context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\", charset=\"UTF-8\"";
              context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          });
      }

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
          c.SwaggerEndpoint(
              $"{Configuration["UrlBase"]}{Configuration["SwaggerEndpoint"]}",
              "MinhaCarteira - API v1");
          c.InjectStylesheet($"{Configuration["UrlBase"]}/css/SwaggerDark.css");
          c.DocExpansion(DocExpansion.None);
      });
  }
  ```

- [ ] **3. Credenciais via secrets (não versionar em `appsettings.json`)**
  - Local (user-secrets):
    ```
    dotnet user-secrets set "Swagger:Usuario" "swagger"
    dotnet user-secrets set "Swagger:Senha" "umaSenhaForte"
    ```
  - Produção (variáveis de ambiente):
    ```
    Swagger__Usuario=swagger
    Swagger__Senha=umaSenhaForte
    ```

### Comportamento esperado

- **Development**: `/swagger` abre direto (sem senha).
- **Production com `Swagger:Usuario`/`Senha` definidos**: o navegador exibe o prompt nativo de usuário/senha
  ao acessar `/swagger` (e o mesmo vale para `/swagger/v1/swagger.json`).
- **Production sem credenciais**: Swagger **não** é exposto (404).
- O "Try it out" continua funcionando: o JWT da API é informado à parte pelo botão **Authorize** (scheme `bearer`).
- http Basic transmite `usuario:senha` em Base64 (não é criptografia) → usar somente sob HTTPS (já garantido por
  `UseHttpsRedirection`/HSTS).

### Notas

- A comparação usa `CryptographicOperations.FixedTimeEquals` para evitar *timing attack*; tamanhos diferentes
  retornam `false` sem lançar exceção.
- Alternativa de organização: extrair o `app.Use(...)` para uma classe
  `SwaggerBasicAuthMiddleware` em `MinhaCarteira.AppServer/Helper/` e registrar com
  `app.UseMiddleware<...>()` (mais testável, porém cria um arquivo novo).
- Endurecimento opcional: guardar um **hash** da senha (ex.: PBKDF2) em `Swagger:SenhaHash` e comparar o hash,
  em vez do texto plano.

---

## Bloco D — Segurança crítica (itens já aplicados)

- [x] **D1.** Backup do token do admin (`__jwtApiOrig`) + `AcessarComoComToken` para o "Sair do Acesso como".
- [x] **D2.** Impersonação restrita a `superadmin,admin` (e correção do `"super,admin"`).
- [x] **D3.** Filtro por organização logada (`FiltrarPorOrganizacaoLogada`) no `PadraoApiController`.
- [x] **D4.** Validação de posse generalizada (`IdValidacaoPosse` / `IgnorarValidacaoPosse`).
- [ ] **D5.** Swagger/CORS por ambiente → ver seção "Swagger ativo em produção..." acima (substitui o passo 1).

---

## Bloco E — Bugfix

Formato de trabalho: **um item por vez + guia manual** (o assistente não implementa; apenas detalha o passo a passo).

### P0 — Crítico

- [x] **E1. "Sair do Acesso como" quebra quando faltam cookies**
  - `MinhaCarteira.AppCliente/Controllers/LoginController.cs` (L214-219)
  - Guard `if (IsNullOrWhiteSpace(id) || IsNullOrWhiteSpace(token))` executa `Logout()` **sem `return`** →
    segue para `new Guid(idUsuarioLogado)` com valor nulo e estoura.
  - Correção: adicionar `return RedirecionarPaginaInicialSite();` dentro do `if`.
  - **Aplicado**: bloco `if` com `return`; o `try/catch (ApiException)` foi removido — passou a ser tratado
    pelo handler global (`app.UseExceptionHandler("/Error")` + `ErrorController`/`ErrorViewModel`).

- [x] **E2. `ImportarNotificacao` marcado como `[AllowAnonymous]`** — *risco assumido*
  - `MinhaCarteira.AppServer/Controllers/ImportarArquivoController.cs` (L34-38)
  - Endpoint de importação de notificação bancária exposto sem autenticação.
  - **Decisão do usuário: manter como está (assumir o risco).** Justificativa: é consumido por integração
    externa sem JWT; a `ImportarArquivoServico` valida que a conta pertence ao `ProprietarioId` informado.
  - Risco remanescente: qualquer um com um par válido `ProprietarioId`+`ContaBancariaId` pode injetar
    movimentos falsos. Se um dia quiser fechar: exigir segredo em header (ex.: `X-Api-Key`) mantendo o
    `[AllowAnonymous]`, ou trocar por JWT (`[Authorize]`).

- [x] **E3. Auditoria e Log vazam dados de todas as organizações**
  - `MinhaCarteira.AppServer/Controllers/AuditoriaController.cs`
  - `MinhaCarteira.AppServer/Controllers/LogController.cs`
  - `[Authorize]` sem `Roles` e filtro de `OrganizacaoId` **comentado** → qualquer usuário autenticado lê tudo.
  - **Aplicado** em dois passos:
    1. `[Authorize(Roles = "superadmin,admin")]` + overrides de posse/org (`IgnorarValidacaoPosse`,
       `PropriedadeValidacaoPosse = "OrganizacaoId"`, `IdValidacaoPosse = IdOrganizacaoUsuarioLogado`,
       `FiltroRegistros = FiltrarPorOrganizacaoLogada`), como no `UsuarioController`.
    2. Refatoração para reuso: `PadraoApiController<TServico>` ficou só com a infraestrutura e foi criado
       `BaseLeituraApiController<TEntidade, TPK, TServico>` com `Navegar` + `ObterPorId` (leitura).
       `BaseApiController` e os controllers de Auditoria/Log herdam dele.
  - Efeitos colaterais corrigidos no caminho: `ObterPorId` do base estava com `<Log>` fixo (500 em todos os
    `GET api/{controller}/{id}`) e a validação de posse do `ObterPorId` tinha sido perdida.

### P1 — Alto

- [ ] **E4. `ConfrontarUsuarioLogadoAttribute` roda após a action**
  - `MinhaCarteira.AppCliente/Filter/ConfrontarUsuarioLogadoAttribute.cs` (L9-27)
  - Valida em `OnResultExecuting` (pós-execução) em vez de `OnActionExecuting`; `routeValues` pode ser nulo.
  - Correção: mover a validação para `OnActionExecuting` + null-check.

- [ ] **E5. `BaseRepositorio.DeletarRange` faz hard delete**
  - `MinhaCarteira.Servico/Repositorio/Base/BaseRepositorio.cs` (L31-42)
  - Usa `Tabela.RemoveRange(objs)` (exclusão física) enquanto o restante do sistema usa soft delete (`Deletado`).
  - A validar: confirmar se é intencional em algum fluxo.

- [x] **E6. `ConfrontarUsuarioLogadoEProprietario` pode lançar NRE**
  - `MinhaCarteira.AppServer/Controllers/Base/PadraoApiController.cs` (L84-104)
  - Usa `resp.Dados.GetValue(...)` sem checar `resp?.Dados == null`.
  - **Aplicado (fail-closed)**: o cast passou a ser `if (retornoOk.Value is not IRespostaServico<TEntidade> resp || resp.Dados is null)
    return FalhaValidacaoUsuario();`. Verificado que o `ServicoHelper` devolve `BadRequest` quando `BemSucedido == false`,
    então o NRE não era reproduzível; ainda assim ficou fechado por segurança.

- [x] **E6.1. `LoginController` (API) — `ObterPorId` valida posse com a propriedade errada**
  - `MinhaCarteira.AppServer/Controllers/LoginController.cs` (`ObterPorId`, `[HttpGet("{id:guid}")]`)
  - Chamava `ConfrontarUsuarioLogadoEProprietario<Usuario>(resposta)` usando o default
    `PropriedadeValidacaoPosse = "ProprietarioId"`, mas `Usuario` só tem `OrganizacaoId` → a checagem era pulada
    → **qualquer usuário autenticado lia qualquer `Usuario` pelo `id`**, incluindo `PasswordHash` (IDOR).
  - **Aplicado**: overrides no `LoginController`
    `PropriedadeValidacaoPosse => "OrganizacaoId"` e `IdValidacaoPosse => IdOrganizacaoUsuarioLogado`.
  - **Decisão de design (centralização)**: `IgnorarValidacaoPosse` deixou de ser override por controller e passou a
    ter o **default no `PadraoApiController`** = `UsuarioEhSuperAdmin` (atribuído no construtor).
    Foram removidos os overrides redundantes de `AuditoriaController`, `LogController`, `UsuarioController` e
    `LoginController`. Comportamento idêntico ao anterior (superadmin global), com menos duplicação.
    Coerente com `ObterFiltroInicial`, que já ignora o filtro de organização para superadmin.
  - **Verificado**: compilação `0 avisos / 0 erros`.
  - Observação: alternativamente `protected virtual bool IgnorarValidacaoPosse => UsuarioEhSuperAdmin;`
    (expressão, igual aos demais virtuais) evitaria o "assign de virtual no construtor" — opcional.

- [ ] **E7. `DefinirUsuarioLogado` ignora `ProprietarioId` nullable**
  - `MinhaCarteira.AppServer/Controllers/Base/PadraoApiController.cs` (L74-82)
  - Só define quando a propriedade não é nullable → `Categoria` e `Arquivo` (`ProprietarioId` é `Guid?`)
    ficam sem dono.
  - A validar.

- [ ] **E8. Endpoints sem validação de posse (IDOR)**
  - `MinhaCarteira.AppServer/Controllers/AgendamentoController.cs` (parcelas: obter/alterar/deletar/pagar/restaurar)
  - `MinhaCarteira.AppServer/Controllers/ContaBancariaController.cs` (`IncrementarPrioridade`,
    `DecrementarPrioridade`, `Reativar`)
  - Operam por id sem `ConfrontarUsuarioLogadoEProprietario`.
  - A validar.

### P2 — Médio

- [ ] **E9. `TotalPaginas` gera página extra**
  - `MinhaCarteira.Servico/Servico/Base/BaseServico.cs` (L91-92)
  - `(total / itens) + 1` soma 1 quando a divisão é exata.
  - Correção: `(int)Math.Ceiling(total / (double)itens)`.

- [ ] **E10. `_MenuLateral.cshtml` acessa `.Value` sem `?.`**
  - `MinhaCarteira.AppCliente/Views/Shared/_MenuLateral.cshtml` (L26, 33, 40, 65)
  - `FirstOrDefault(...).Value` → NRE quando a claim não existe. Correção: `FirstOrDefault(...)?.Value`.

- [ ] **E11. `Guid.Parse` sem validação**
  - `MinhaCarteira.AppServer/Controllers/ImportarArquivoController.cs` (L21-27)
  - `Guid.Parse(IdUsuarioLogado)` estoura com entrada inválida → usar `Guid.TryParse`.

- [ ] **E12. `NotImplementedException` após processar**
  - `MinhaCarteira.Servico/Servico/AgendamentoServico.cs` (L241-280)
  - `ConciliarParcelasAPartirDashboardMonitor` executa a lógica e sempre termina lançando exceção.

- [ ] **E13. Ordenação `"id"` em minúsculo**
  - `MinhaCarteira.Servico/Repositorio/Base/BaseRepositorio.cs` (L87-103)
  - `criterio.Ordenacao = "id"` vs propriedade `Id`; depende do comparador ser case-insensitive.
  - A validar.

- [ ] **E14. `DateTime.Now` na expiração do token**
  - `MinhaCarteira.Servico/Servico/LoginServico.cs` (L117, L166)
  - `DataExpericaoToken = DateTime.Now...` divergindo do `UtcNow` usado em `TokenServico`.
  - A validar.

### Descartados na varredura (não são bugs)

- Role `usuariologado` — é adicionada também no login normal (`LoginServico`).
- `Utils.Excluir` vs `Utils.SalvarAsync` — `Extensao` já vem com o ponto incluído.
- `"super,admin"` — já corrigido.
