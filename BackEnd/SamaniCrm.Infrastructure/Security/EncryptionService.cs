using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Security;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string Encrypt(string plainText, string key);
    string Decrypt(string cipherText, string key);
    string Hash(string input);
    bool VerifyHash(string input, string hash);
    string GenerateRandomKey(int length = 32);
    string GenerateRandomToken();
}

public class EncryptionService : IEncryptionService
{
    private readonly string _defaultKey;
    private readonly ILogger<EncryptionService> _logger;

    // AES-256 configuration
    private const int KeySize = 256;
    private const int BlockSize = 128;
    private const int Iterations = 10000;

    public EncryptionService(
        IOptions<EncryptionSettings> settings,
        ILogger<EncryptionService> logger)
    {
        _defaultKey = settings.Value.EncryptionKey
            ?? throw new InvalidOperationException("Encryption key not configured");
        _logger = logger;

        // Validate key length
        if (!IsValidKey(_defaultKey))
        {
            throw new InvalidOperationException("Encryption key must be 32 bytes (256 bits) for AES-256");
        }
    }

    public string Encrypt(string plainText)
    {
        return Encrypt(plainText, _defaultKey);
    }

    public string Decrypt(string cipherText)
    {
        return Decrypt(cipherText, _defaultKey);
    }

    public string Encrypt(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = GetValidKey(key);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Prepend IV to encrypted data
            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption failed");
            throw new CryptographicException("Encryption failed", ex);
        }
    }

    public string Decrypt(string cipherText, string key)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = GetValidKey(key);

            // Extract IV from the beginning
            var iv = new byte[BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            throw new CryptographicException("Decryption failed", ex);
        }
    }

    public string Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Generate random salt
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Create hash
        using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        // Combine salt and hash
        var hashBytes = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyHash(string input, string storedHash)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(storedHash))
            return false;

        try
        {
            var hashBytes = Convert.FromBase64String(storedHash);

            // Extract salt
            var salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, salt.Length);

            // Compute hash with same salt
            using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            // Compare
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + salt.Length] != hash[i])
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateRandomKey(int length = 32)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string GenerateRandomToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private bool IsValidKey(string key)
    {
        try
        {
            var keyBytes = Convert.FromBase64String(key);
            return keyBytes.Length == 32;
        }
        catch
        {
            // Try as raw string
            return Encoding.UTF8.GetBytes(key).Length == 32;
        }
    }

    private byte[] GetValidKey(string key)
    {
        try
        {
            var keyBytes = Convert.FromBase64String(key);
            if (keyBytes.Length == 32)
                return keyBytes;
        }
        catch
        {
            // Not base64, use as raw
        }

        // Hash the key to get consistent 32 bytes
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }
}

// Settings
public class EncryptionSettings
{
    public string EncryptionKey { get; set; } = string.Empty;
    public string HashSalt { get; set; } = string.Empty;
}
