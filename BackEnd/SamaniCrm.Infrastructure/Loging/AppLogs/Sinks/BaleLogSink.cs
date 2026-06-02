using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SamaniCrm.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.Loging.AppLogs.Sinks;

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

    public async Task WriteAsync(AppLogEntry entry)
    {
        if (entry.Level < _minLevel)
            return;

        if (string.IsNullOrEmpty(_botToken) || string.IsNullOrEmpty(_chatId))
        {
            throw new Exception("Bale log config not set!");
        }

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
            ⭕ Controller= {entry.ControllerName}
            🔰 Action= {entry.ActionName}
            {entry.RequestPath}
            
            {(string.IsNullOrEmpty(entry.ExceptionDetails) ? "" : $"💥 ```\n{entry.ExceptionDetails}\n```")}
            """;

        var url = $"https://tapi.bale.ai/bot{_botToken}/sendMessage";
        // var r = new HttpRequestMessage();
        // r.Headers.Add("ContentType", "application/json");
        // r.Method = HttpMethod.Post;
        // r.RequestUri =new Uri(url);
        //var response= await _httpClient.SendAsync(r); 
        var body = new
        {
            chat_id = _chatId,
            text = message,
            parse_mode = "Markdown"
        };
        var response = await _httpClient.PostAsJsonAsync(url,body);
        //Console.WriteLine($"Url:{url}",url);
       // Console.WriteLine($"Body:{body}",JsonSerializer.Serialize(body));
       // Console.WriteLine(await response.Content.ReadAsStringAsync());
       // Console.WriteLine(response);
    }
}