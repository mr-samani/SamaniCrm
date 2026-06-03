namespace SamaniCrm.Application.Features.Tenants.Interfaces;

public interface ITenantDatabaseService
{
    Task CreateDatabaseAsync(string server, string databaseName, string username,
        string password, CancellationToken cancellation);
    Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellation);
    string GenerateConnectionString(string server, string database, string username, string password);
    string EncryptConnectionString(string connectionString);
    string DecryptConnectionString(string encryptedConnectionString);
    Task RunMigrationsAsync(string connectionString,Guid tenantId, CancellationToken cancellation);
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string? GetEncryptedConnectionString(Guid? tenantId);
    string? GetConnectionString(Guid? tenantId);
}
