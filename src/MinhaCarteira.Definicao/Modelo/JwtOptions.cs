namespace MinhaCarteira.Definicao.Modelo;

public class JwtOptions
{
    public const string Secao = "Jwt";
    public string Segredo { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
