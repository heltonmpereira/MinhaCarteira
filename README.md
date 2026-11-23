# MinhaCarteira

Sistema de controle financeiro pessoal: API (MinhaCarteira.AppServer) + App Cliente (MinhaCarteira.AppCliente).

## Configuração do ambiente local

### Pré-requisitos
- .NET SDK 11
- PostgreSQL (Supabase)

### Segredos locais (User Secrets)

O projeto `MinhaCarteira.AppServer` já possui `UserSecretsId` definido no `.csproj`,
portanto NÃO execute `dotnet user-secrets init`. Basta cadastrar os valores:

    cd src/MinhaCarteira.AppServer
    dotnet user-secrets set "Jwt:Segredo" "<chave-gerada-abaixo>"

    dotnet user-secrets set "ConnectionStrings:Default" "User Id=...;Password=...;Server=...;Port=5432;Database=postgres"
    
    dotnet user-secrets set "Repositorio:Chave" "<32 caracteres>"
    
    dotnet user-secrets set "Repositorio:IV" "<16 caracteres>"

    dotnet user-secrets list

    dotnet user-secrets remove "Jwt:Segredo"

### Gerar chave JWT de 256 bits (compatível com PowerShell 5.1)
$b = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($b)
$rng.Dispose()
[Convert]::ToBase64String($b)

ou
    $bytes = New-Object byte[] 32
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    [Convert]::ToBase64String($bytes)

> PowerShell 5.1 não possui `RandomNumberGenerator::Fill`; use o script acima (ou PowerShell 7 / OpenSSL).

### Produção

Usar variáveis de ambiente com separador `__`:
`Jwt__Segredo`, `ConnectionStrings__Default`.

- Abra o IIS Manager e selecione o site da API (não o servidor).
- Duplo clique em Configuration Editor .
- Em Section , escolha`system.webServer/aspNetCore` .
- Clique na linha`environmentVariables` → botão`...` → Add :
- `name` :`Jwt__Segredo`
- `value` : o mesmo valor que está no seu User Secrets / no Render
- Apply (fecha o dialog).
- Recicle o Application Pool.