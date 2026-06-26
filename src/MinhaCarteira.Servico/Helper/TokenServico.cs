using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MinhaCarteira.Definicao.Modelo.Usuario;

namespace MinhaCarteira.Servico.Helper;

public static class TokenServico
{
    public const string Segredo = "fedaf7d8863b48e197b9287d492b708e";

    public static string GerarToken(UsuarioToken dadosUsuario, Guid organizacaoId)
    {
        var claims = new List<Claim> {
        new("UsuarioId", dadosUsuario.Id.ToString()),
        new("OrganizacaoId", organizacaoId.ToString()),
        new(ClaimTypes.Name, dadosUsuario.Nome),
        new(ClaimTypes.Email, dadosUsuario.Email),
    };

        claims.AddRange(dadosUsuario.Roles
            .Select(s => new Claim(ClaimTypes.Role, s.Trim().ToLower())));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Segredo);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.Now.AddMonths(1),
            Expires = dadosUsuario.DataExpericaoToken,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            TokenType = "Bearer"
        };

        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}