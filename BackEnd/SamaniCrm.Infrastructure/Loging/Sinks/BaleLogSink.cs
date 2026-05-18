using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SamaniCrm.Domain.Entities;
using System.Net.Http.Json;

namespace SamaniCrm.Infrastructure.Loging.Sinks;

public class BaleLogSink : ILogSink
{
    private readonly HttpClient _httpClient;
    private readonly string _botToken;
    private readonly string _chatId;
    private readonly LogLevel _minLevel;

    public string Name => "Bale";

    public BaleLogSink(IConfiguration configuration)
    {
        _botToken = configuration["Logging:Bale:BotToken"] ?? "";
        _chatId = configuration["Logging:Bale:ChatId"] ?? "";
        _httpClient = new HttpClient();

        var minLevelStr = configuration["Logging:Bale:MinLevel"] ?? "Error";
        _minLevel = Enum.Parse<LogLevel>(minLevelStr);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task WriteAsync(LogEntry entry)
    {
        if (entry.Level < _minLevel)
            return;

        var emoji = entry.Level switch
        {
            LogLevel.Critical => "🔴",
            LogLevel.Error => "❌",
            LogLevel.Warning => "⚠️",
            _ => "ℹ️"
        };

        var message = $"""
            {emoji} *{entry.Level}*
            
            📝 {entry.Message}
            
            🕐 {entry.Timestamp:yyyy-MM-dd HH:mm:ss} UTC
            👤 {entry.UserName ?? "Anonymous"}
            🌐 {entry.IpAddress}
            📍 {entry.Source}
            
            {(string.IsNullOrEmpty(entry.ExceptionDetails) ? "" : $"💥 ```\n{entry.ExceptionDetails}\n```")}
            """;

        var url = $"https://api.bale.ai/bot{_botToken}/sendMessage";

      var response=   await _httpClient.PostAsJsonAsync(url, new
        {
            chat_id = _chatId,
            text = message,
            parse_mode = "Markdown"
        });

        Console.WriteLine(response);
    }
}