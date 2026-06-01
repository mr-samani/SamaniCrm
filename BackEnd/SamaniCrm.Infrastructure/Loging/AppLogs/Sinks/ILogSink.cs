using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Loging.AppLogs.Sinks;

public interface ILogSink
{
    string Name { get; }
    Task WriteAsync(AppLogEntry entry);
    Task InitializeAsync();
}
