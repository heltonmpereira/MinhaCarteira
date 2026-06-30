using System;
using Microsoft.AspNetCore.Identity;

namespace MinhaCarteira.Servico.Helper;

public static class Criptografia
{
    private static readonly PasswordHasher<object> _passwordHasher = new();

    public static string CriptografarTexto(this string texto)
    {
        return texto == null 
            ? throw new ArgumentNullException(nameof(texto)) 
            : _passwordHasher.HashPassword(new object(), texto);
    }

    public static bool VerificarHashSenha(this string senhaCifrada, string senha)
    {
        ArgumentNullException.ThrowIfNull(senha);

        if (senhaCifrada == null)
            return false;

        var result = _passwordHasher.VerifyHashedPassword(new object(), senhaCifrada, senha);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}