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
        builder.ToTable("LogSettings","logs");
        builder.HasIndex(e => e.TenantId).IsUnique();

        builder.Property(e => e.EnabledLevels)
            .HasConversion<string>();
        builder.Property(e => e.EnabledSinks)
            .HasConversion<string>();
    }
}



public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
{
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.ToTable("LogEntries","logs");

        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.Level);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CorrelationId);

        // Index ترکیبی برای کوئری‌های رایج
        builder.HasIndex(e => new { e.TenantId, e.Timestamp, e.Level });

        builder.Property(e => e.Level)
            .HasConversion<string>();


        // Value Converter برای Dictionary
        var dictionaryConverter = new ValueConverter<Dictionary<string, object>?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => v == null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null)
        );

    

        // ✅ Value Comparer برای مقایسه صحیح Dictionary
        var dictionaryComparer = new ValueComparer<Dictionary<string, object>?>(
            (left, right) => JsonSerializer.Serialize(left, (JsonSerializerOptions?)null)
                           == JsonSerializer.Serialize(right, (JsonSerializerOptions?)null),
            v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => v == null ? null! : JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
        );

        builder.Property(e => e.ExtraData)
                .HasConversion(dictionaryConverter)
                .Metadata.SetValueComparer(dictionaryComparer);

    }
}