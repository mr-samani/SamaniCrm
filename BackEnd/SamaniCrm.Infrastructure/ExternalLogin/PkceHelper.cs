using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.ExternalLogin;

public static class PkceHelper
{
    // RFC 7636: code_verifier length between 43 and 128 chars
    public static string GenerateCodeVerifier(int length = 64)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return Base64UrlEncode(bytes);
    }

    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.ASCII.GetBytes(codeVerifier);
        var hash = sha256.ComputeHash(bytes);
        return Base64UrlEncode(hash);
    }

    public static string GenerateState(int length = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return Base64UrlEncode(bytes);
    }

    public static string GenerateNonce(int length = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] buffer)
    {
        return Convert.ToBase64String(buffer)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}