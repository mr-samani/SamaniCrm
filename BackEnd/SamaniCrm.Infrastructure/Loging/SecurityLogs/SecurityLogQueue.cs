using Microsoft.AspNetCore.Http;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
namespace SamaniCrm.Infrastructure.Loging.SecurityLogs;

public sealed class SecurityLogQueue : ISecurityLogQueue
{
    private readonly Channel<SecurityLogDto> _channel;

    public SecurityLogQueue()
    {
        var options = new BoundedChannelOptions(100_000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        };

        _channel = Channel.CreateBounded<SecurityLogDto>(options);
    }

    public ValueTask EnqueueAsync(SecurityLogDto log)
    {
        return _channel.Writer.WriteAsync(log);
    }

    public ChannelReader<SecurityLogDto> Reader => _channel.Reader;
}