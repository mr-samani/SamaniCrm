using Duende.IdentityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class TenantLogSettingConfiguration : IEntityTypeConfiguration<TenantLogSetting>
{
    public void Configure(EntityTypeBuilder<TenantLogSetting> builder)
    {
        builder.ToTable("LogSettings", "logs");
        builder.HasIndex(e => e.TenantId).IsUnique();

        builder.Property(e => e.EnabledLevels)
            .HasConversion<string>();
        builder.Property(e => e.EnabledSinks)
            .HasConversion<string>();
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "logs");
    }
}


public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
{  // ۱. تنظیمات JsonSerializerOptions با ترتیب کلیدهای قطعی
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        // ✅ این مهم‌ترین بخش است - ترتیب کلیدها را تضمین می‌کند
        PropertyNameCaseInsensitive = true
    };
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.ToTable("LogEntries", "logs");

        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.Level);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CorrelationId);

        // Index ترکیبی برای کوئری‌های رایج
        builder.HasIndex(e => new { e.TenantId, e.Timestamp, e.Level });


        var dictionaryConverter = new ValueConverter<Dictionary<string, object>?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
            v => string.IsNullOrEmpty(v)
                ? null
                : JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonOptions)
        );

        var dictionaryComparer = new ValueComparer<Dictionary<string, object>?>(
            (d1, d2) => JsonSerializer.Serialize(d1, JsonOptions) == JsonSerializer.Serialize(d2, JsonOptions),
            d => d == null ? 0 : JsonSerializer.Serialize(d, JsonOptions).GetHashCode(),
            d => d == null
                ? null
                : JsonSerializer.Deserialize<Dictionary<string, object>>(
                    JsonSerializer.Serialize(d, JsonOptions),
                    JsonOptions)
        );

        builder.Property(e => e.ExtraData)
            .HasConversion(dictionaryConverter)
            .Metadata.SetValueComparer(dictionaryComparer);

          

    }
}