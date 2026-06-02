namespace SamaniCrm.Core.Shared.Logging.Dtos;

public class AppLogStatsDto
{
    public int TotalCount { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Dictionary<string, int> LevelCounts { get; set; } = new();
}
