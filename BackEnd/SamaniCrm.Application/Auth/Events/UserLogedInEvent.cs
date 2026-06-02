using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Auth.Events;

public record UserLogedInEvent(
    Guid userId,
    string userName,
    Guid? tenantId,
    string message
    ):INotification
{
}
