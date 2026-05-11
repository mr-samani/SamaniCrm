using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Security;

public interface IConnectionStringEncryptor
{
    string Encrypt(string connectionString);
    string Decrypt(string encryptedConnectionString);
    SqlConnectionStringBuilder ParseConnectionString(string connectionString);
    bool ValidateConnectionString(string connectionString);
}

public class ConnectionStringEncryptor : IConnectionStringEncryptor
{
    private readonly IEncryptionService _encryption;
    private readonly ILogger<ConnectionStringEncryptor> _logger;

    public ConnectionStringEncryptor(
        IEncryptionService encryption,
        ILogger<ConnectionStringEncryptor> logger)
    {
        _encryption = encryption;
        _logger = logger;
    }

    public string Encrypt(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return string.Empty;

        // Remove password before encryption for logging
        var builder = new SqlConnectionStringBuilder(connectionString);
        var password = builder.Password;
        builder.Password = "***HIDDEN***";

        var encrypted = _encryption.Encrypt(connectionString);

        _logger.LogDebug("Connection string encrypted (password hidden: {Builder})", builder.ConnectionString);

        return encrypted;
    }

    public string Decrypt(string encryptedConnectionString)
    {
        if (string.IsNullOrEmpty(encryptedConnectionString))
            return string.Empty;

        return _encryption.Decrypt(encryptedConnectionString);
    }

    public SqlConnectionStringBuilder ParseConnectionString(string connectionString)
    {
        try
        {
            return new SqlConnectionStringBuilder(connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid connection string format");
            throw new ArgumentException("Invalid connection string format", ex);
        }
    }

    public bool ValidateConnectionString(string connectionString)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            // Check required fields
            if (string.IsNullOrEmpty(builder.DataSource))
                return false;
            if (string.IsNullOrEmpty(builder.InitialCatalog))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}