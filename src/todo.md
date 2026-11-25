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
