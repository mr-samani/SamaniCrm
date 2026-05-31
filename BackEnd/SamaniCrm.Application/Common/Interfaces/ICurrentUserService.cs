using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string Lang { get; }

        bool IsDelegated { get; }
        Guid? DelegatorId { get; }
        bool IsAuthenticated { get; }
    }
}
