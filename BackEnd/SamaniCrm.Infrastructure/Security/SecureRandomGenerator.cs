using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Security;

public interface ISecureRandomGenerator
{
    string GenerateToken(int length = 32);
    string GenerateAlphanumericToken(int length = 32);
    string GenerateNumericToken(int length = 6);
    byte[] GenerateBytes(int length);
    Guid GenerateGuid();
}

public class SecureRandomGenerator : ISecureRandomGenerator
{
    public string GenerateToken(int length = 32)
    {
        var bytes = GenerateBytes(length);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "")
            .Substring(0, length);
    }

    public string GenerateAlphanumericToken(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new char[length];
        var bytes = GenerateBytes(length);

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }

        return new string(result);
    }

    public string GenerateNumericToken(int length = 6)
    {
        var bytes = GenerateBytes(length);
        var result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = (char)('0' + (bytes[i] % 10));
        }

        return new string(result);
    }

    public byte[] GenerateBytes(int length)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }

    public Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }
}