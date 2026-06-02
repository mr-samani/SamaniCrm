using MediatR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using System.Net;

namespace SamaniCrm.Application.Auth.Events;

public class UserLogedInSecurityLogHandler : INotificationHandler<UserLogedInEvent>
{

    private readonly ISecurityLogFactory _securityLogFactory;
    private readonly ISecurityLogQueue _securityLogQueue;

    public UserLogedInSecurityLogHandler(ISecurityLogFactory securityLogFactory, ISecurityLogQueue securityLogQueue)
    {
        _securityLogFactory = securityLogFactory;
        _securityLogQueue = securityLogQueue;
    }


    public async Task Handle(UserLogedInEvent notification, CancellationToken cancellationToken)
    {
        var data = _securityLogFactory.Create(SecurityEventType.LoginSuccess, LogLevel.Information, true, HttpStatusCode.OK, notification.message);
        await _securityLogQueue.EnqueueAsync(data);
    }
}
