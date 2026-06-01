using Microsoft.Extensions.Configuration;
using SamaniCrm.Domain.Entities;
using System.Net.Http.Json;

namespace SamaniCrm.Infrastructure.Loging.AppLogs.Sinks;

public class ExternalApiLogSink : ILogSink
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _apiKey;

    public string Name => "ExternalApi";

    public ExternalApiLogSink(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _apiUrl = configuration["Logging:ExternalApi:Url"] ?? "";
        _apiKey = configuration["Logging:ExternalApi:ApiKey"] ?? "";
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task WriteAsync(AppLogEntry entry)
    {
        var payload = new
        {
            entry.TenantId,
            Level = entry.Level.ToString(),
            entry.Message,
            entry.ExceptionDetails,
            entry.Source,
            entry.UserId,
            entry.CorrelationId,
            entry.Timestamp
        };

        try
        {
            await _httpClient.PostAsJsonAsync(_apiUrl, payload);
        }
        catch
        {
            // Silent fail برای External API
        }
    }
}