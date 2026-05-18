namespace SamaniCrm.Core.Shared.Logging.Dtos;

public class TenantLogSettingDto
{
    public Guid? TenantId { get; set; }
    public bool IsEnabled { get; set; }
    public LogLevelMask EnabledLevels { get; set; }
    public LogSinkMask EnabledSinks { get; set; }
    public int? RetentionDays { get; set; }
    public string? CustomSettings { get; set; }
}