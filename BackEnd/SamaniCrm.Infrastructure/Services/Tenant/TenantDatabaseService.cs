using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Infrastructure.Security;

namespace SamaniCrm.Infrastructure.Services.TenantService;

public class TenantDatabaseService : ITenantDatabaseService
{
    private readonly ILogger<TenantDatabaseService> _logger;
    private readonly IEncryptionService _encryption;
    private readonly string _encryptionKey;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IServiceProvider _serviceProvider;


    public TenantDatabaseService(
        ILogger<TenantDatabaseService> logger,
        IEncryptionService encryption,
        ICurrentUserService currentUser,
        ICurrentTenant currentTenant,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _encryption = encryption;
        _encryptionKey = Environment.GetEnvironmentVariable("CONNECTION_STRING_ENCRYPTION_KEY")
            ?? throw new InvalidOperationException("Encryption key not configured");
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _serviceProvider = serviceProvider;
    }

    public async Task CreateDatabaseAsync(string server, string databaseName,
        string username, string password, CancellationToken cancellation)
    {
        var masterConnectionString = GenerateConnectionString(server, "master", username, password);

        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync(cancellation);

        var createDbCommand = $@"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                CREATE DATABASE [{databaseName}];
            END";

        await using var cmd = new SqlCommand(createDbCommand, connection);
        await cmd.ExecuteNonQueryAsync(cancellation);

        _logger.LogInformation("Database {DatabaseName} created or already exists", databaseName);
    }

    public async Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellation)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateConnectionString(string server, string database, string username, string password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = username,
            Password = password,
            TrustServerCertificate = true,
            ConnectTimeout = 30,
            MultipleActiveResultSets = true,
            Encrypt = true
        };

        return builder.ConnectionString;
    }

    public string EncryptConnectionString(string connectionString)
    {
        return _encryption.Encrypt(connectionString, _encryptionKey);
    }

    public string DecryptConnectionString(string encryptedConnectionString)
    {
        return _encryption.Decrypt(encryptedConnectionString, _encryptionKey);
    }

    public async Task RunMigrationsAsync(string connectionString, Guid tenantId, CancellationToken cancellation)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        using var context = new ApplicationDbContext(optionsBuilder.Options, _currentUser, _currentTenant, _serviceProvider);
        await context.Database.MigrateAsync(cancellation);
    }

    public string Encrypt(string plainText)
    {
        return _encryption.Encrypt(plainText, _encryptionKey);
    }

    public string Decrypt(string cipherText)
    {
        return _encryption.Decrypt(cipherText, _encryptionKey);
    }
}
