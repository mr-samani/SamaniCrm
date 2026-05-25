using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Core.Shared.Logging;

[Flags]
public enum LogLevelMask
{
    None            = 0,
    Trace           = 1 << 0,    // 1
    Debug           = 1 << 1,    // 2
    Information     = 1 << 2,    // 4
    Warning         = 1 << 3,    // 8
    Error           = 1 << 4,    // 16
    Critical        = 1 << 5,    // 32
    All = Trace | Debug | Information | Warning | Error | Critical
}

[Flags]
public enum LogSinkMask
{
    None            = 0,
    File            = 1 << 0,
    Database        = 1 << 1,
    Telegram        = 1 << 2,
    Bale            = 1 << 3,
    ExternalApi     = 1 << 4,
    All = File | Database | Telegram | ExternalApi | Bale
}
