using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;

 
public interface ITenantUniquenessChecker
{
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellation);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellation);
    Task<bool> ExistsByUserEmailAsync(string email, CancellationToken cancellation);
}
 
