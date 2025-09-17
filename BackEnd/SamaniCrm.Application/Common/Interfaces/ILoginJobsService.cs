using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces;
public interface ILoginJobsService
{
    public Task ReleaseExpiredLocksAsync(PerformContext context, CancellationToken cancellationToken);
    public Task ReleaseExpiredLocksAsync(PerformContext context);

}